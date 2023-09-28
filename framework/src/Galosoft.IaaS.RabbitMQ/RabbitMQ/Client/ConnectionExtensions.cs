using CommunityToolkit.Diagnostics;
using FreeRedis;
using Galosoft.IaaS.Core;
using Galosoft.IaaS.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ.Client
{
    /// <summary>
    /// extension of IConnectionFactory <see cref="IConnectionFactory"/>
    /// </summary>
    public static class ConnectionExtensions
    {
        private static IDictionary<string, IConnection> _pool = new Dictionary<string, IConnection>();
        private static IDictionary<string, List<string>> _subscribers = new Dictionary<string, List<string>>();//新建：订阅者池 galo@2022-2-9 10:35:29 
        private static object _lock = new object();
        private static Policy _retryPolicy = Policy.Handle<Exception>()
                    .WaitAndRetry(5
                    , retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    , (exception, timeSpan, retryCount, context) => Tracer.Info($"重试{retryCount}，{exception}", "消息发送"));
        /// <summary>
        /// 各个消息类的MessageId、MessageKey对应的属性
        /// </summary>
        private static IDictionary<Type, IDictionary<string, PropertyInfo[]>> _basicPropertyMappings = new Dictionary<Type, IDictionary<string, PropertyInfo[]>>();

        #region Distributor.
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="routeKey"></param>
        /// <param name="broadcast"></param>
        /// <param name="arguments"></param>
        /// <param name="serializer"></param>
        /// <param name="msgs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Distribute<T>(this IConnectionFactory factory, string routeKey = "", bool broadcast = false, QueueArgument? arguments = null, IObjectSerializer? serializer = null, params T[] msgs)
        {
            if (string.IsNullOrEmpty(routeKey))
                throw new ArgumentNullException(nameof(routeKey));

            if (arguments == null)
                arguments = new QueueArgument();

            Dictionary<string, object>? args = null;

            if (!arguments.ResourceDeclared)
                args = GetArgsByQueueArgument(routeKey, arguments);

            var rt = _retryPolicy.ExecuteAndCapture(() => DistributeInternal(factory, routeKey, broadcast, args, arguments.ResourceDeclared, serializer, msgs));
            if (rt.Outcome == OutcomeType.Failure)
                Tracer.Error(rt.FinalException, "消息发送");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="msg"></param>
        /// <param name="routeKey"></param>
        /// <param name="broadcast"></param>
        /// <param name="arguments"></param>
        /// <param name="serializer"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Distribute<T>(this IConnectionFactory factory, T msg, string routeKey = "", bool broadcast = false, QueueArgument? arguments = null, IObjectSerializer? serializer = null)
        {
            if (arguments == null)
                arguments = new QueueArgument();

            Dictionary<string, object>? args = null;

            if (!arguments.ResourceDeclared)
                args = GetArgsByQueueArgument(routeKey, arguments);

            DistributeInternal(factory, routeKey, broadcast, args, arguments.ResourceDeclared, serializer, new[] { msg });
        }
        #endregion

        #region Generic handler.
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="property"></param>
        /// <param name="handler"></param>
        /// <param name="queue"></param>
        /// <param name="exchange"></param>
        /// <param name="arguments"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Handle<T>(
            this IConnectionFactory factory,
            HandlerProperty property,
            Func<T, Task<bool>> handler,
            string queue = "",
            string exchange = "",
            QueueArgument arguments = null,
            IObjectSerializer? serializer = null)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            if (string.IsNullOrEmpty(queue))
                throw new ArgumentNullException(nameof(queue));

            if (serializer is null)
                serializer = new MicrosoftJsonSerializer();

            var routingKey = queue;
            if (!string.IsNullOrEmpty(exchange))
                routingKey = $"{exchange}:{queue}";

            Trace.WriteLine($"\t当前 【{routingKey}】", "“消息处理”>");

            var cnn = GetOrCreateConnection(factory, queue);
            var channel = cnn.CreateModel();

            channel.BasicQos(0, property.PrefetchCount, false);//新增：根据消费速度设置预取数量 galo@2022-1-18 11:40:40

            if (!arguments.ResourceDeclared)
            {
                var args = GetArgsByQueueArgument(queue, arguments);
                channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: args);//修改：该队列改为不持久化 galo@2022-2-9 10:42:51

                #region 订阅：接收发布到该交换机的消息 galo@2022-2-9 10:43:44
                if (!string.IsNullOrEmpty(exchange))
                {
                    //1.声明一个交换机 防止该交换机还未创建
                    channel.ExchangeDeclare(exchange, ExchangeType.Fanout, true, false);
                    //2.声明一个队列并绑定到该交换机
                    channel.QueueBind(queue, exchange, string.Empty, null);
                }
                #endregion
            }

            if (property.AsyncEnabled)//同步
            {
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (sender, e) => await HandleIdempotentlyInternalAsync(channel, e, property, handler, queue, arguments, serializer);
                channel.BasicConsume(queue, false, consumer);
            }
            else
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (sender, e) => await HandleIdempotentlyInternalAsync(channel, e, property, handler, queue, arguments, serializer);
                channel.BasicConsume(queue, false, consumer);
            }
        }
        #endregion

        #region Type handler.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="property"></param>
        /// <param name="handler"></param>
        /// <param name="msgType"></param>
        /// <param name="queue"></param>
        /// <param name="exchange"></param>
        /// <param name="arguments"></param>
        /// <param name="serializer"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Handle(
            this IConnectionFactory factory,
            HandlerProperty property,
            Func<object, Task<bool>> handler,
            Type msgType,
            string queue = "",
            string exchange = "",
            QueueArgument arguments = null,
            IObjectSerializer? serializer = null)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            if (string.IsNullOrEmpty(queue))
                throw new ArgumentNullException(nameof(queue));

            if (serializer is null)
                serializer = new MicrosoftJsonSerializer();

            var routingKey = queue;
            if (!string.IsNullOrEmpty(exchange))
                routingKey = $"{exchange}:{queue}";

            Trace.WriteLine($"\t当前 【{routingKey}】", "“消息处理”>");

            var cnn = GetOrCreateConnection(factory, queue);
            var channel = cnn.CreateModel();

            channel.BasicQos(0, property.PrefetchCount, false);//新增：根据消费速度设置预取数量 galo@2022-1-18 11:40:40

            if (!arguments.ResourceDeclared)
            {
                var args = GetArgsByQueueArgument(queue, arguments);
                channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: args);//修改：该队列改为不持久化 galo@2022-2-9 10:42:51

                #region 订阅：接收发布到该交换机的消息 galo@2022-2-9 10:43:44
                if (!string.IsNullOrEmpty(exchange))
                {
                    //1.声明一个交换机 防止该交换机还未创建
                    channel.ExchangeDeclare(exchange, ExchangeType.Fanout, true, false);
                    //2.声明一个队列并绑定到该交换机
                    channel.QueueBind(queue, exchange, string.Empty, null);
                }
                #endregion
            }

            if (property.AsyncEnabled)//同步
            {
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (sender, e) => await HandleIdempotentlyInternalAsync(channel, e, property, handler, queue, arguments, serializer);
                channel.BasicConsume(queue, false, consumer);
            }
            else
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (sender, e) => await HandleIdempotentlyInternalAsync(channel, e, property, handler, queue, arguments, serializer);
                channel.BasicConsume(queue, false, consumer);
            }
        }
        #endregion

        #region internal methods.
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="routeKey"></param>
        /// <param name="broadcast"></param>
        /// <param name="arguments"></param>
        /// <param name="declared"></param>
        /// <param name="serializer"></param>
        /// <param name="msgs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void DistributeInternal<T>(this IConnectionFactory factory, string routeKey = "", bool broadcast = false, Dictionary<string, object>? arguments = null, bool declared = false, IObjectSerializer? serializer = null, params T[] msgs)
        {
            if (string.IsNullOrEmpty(routeKey))
                throw new ArgumentNullException(nameof(routeKey));

            if (serializer is null)
                serializer = new MicrosoftJsonSerializer();

            var cnn = GetOrCreateConnection(factory, routeKey);
            using (var channel = cnn.CreateModel())
            {
                if (!declared)
                {
                    if (broadcast)
                    {
                        channel.ExchangeDeclare(routeKey, ExchangeType.Fanout, true, false);//新增：如果广播的话发送至交换机 galo@2022-2-21 16:26:21
                    }
                    else
                    {
                        channel.QueueDeclare(routeKey, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                    }
                }

                var props = channel.CreateBasicProperties();
                props.Persistent = true;
                props.ContentEncoding = Encoding.UTF8.EncodingName;//galo@2023-5-30 14:27:34 设置某些消息的识别属性 例如编码、媒体类型、头、类类型名
                props.ContentType = "application/json";
                props.Headers = arguments;
                props.Type = typeof(T).FullName;

                var type = typeof(T);

                TryCreateOrUpdateBasicPropertyMappings(type);

                foreach (var msg in msgs)
                {
                    TrySetBasicProperties(props, msg);

                    var json = serializer.Serialize(msg);
                    channel.BasicPublish(broadcast ? routeKey : string.Empty, routeKey, props, Encoding.UTF8.GetBytes(json));

                    Trace.WriteLine($"\t{(broadcast ? "交换机" : "队列")}：【{routeKey}】,长度：【{Encoding.UTF8.GetByteCount(json)}】", $"“消息发送”>");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="msg"></param>
        /// <param name="routeKey"></param>
        /// <param name="broadcast"></param>
        /// <param name="arguments"></param>
        /// <param name="declared"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete]
        internal static void DistributeInternal<T>(this IConnectionFactory factory, T msg, string routeKey = "", bool broadcast = false, Dictionary<string, object>? arguments = null, bool declared = false)
        {
            if (string.IsNullOrEmpty(routeKey))
                throw new ArgumentNullException(nameof(routeKey));

            var cnn = GetOrCreateConnection(factory, routeKey);
            using (var channel = cnn.CreateModel())
            {
                if (declared)
                {
                    if (broadcast)
                    {
                        channel.ExchangeDeclare(routeKey, ExchangeType.Fanout, true, false);//新增：如果广播的话发送至交换机 galo@2022-2-21 16:26:21
                    }
                    else
                    {
                        channel.QueueDeclare(routeKey, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                    }
                }

                var props = channel.CreateBasicProperties();
                props.Persistent = true;

                #region System.Text.Json
                var opt = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false,//新增：设置大小写不敏感 galo@2022-2-21 09:57:49，
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,//新增：设置使用驼峰形式属性名 提交 galo@2022-2-23 16:08:28
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                var json = System.Text.Json.JsonSerializer.Serialize(msg, opt);
                #endregion

                //var json = JsonConvert.SerializeObject(msg);
                channel.BasicPublish(broadcast ? routeKey : string.Empty, routeKey, props, Encoding.UTF8.GetBytes(json));
                Trace.WriteLine($"\t{(broadcast ? "交换机" : "队列")}：【{routeKey}】,长度：【{Encoding.UTF8.GetByteCount(json)}】", $"“消息发送”>");
            }
        }

        internal static async Task HandleIdempotentlyInternalAsync<T>(IModel channel, BasicDeliverEventArgs e, HandlerProperty property, Func<T, Task<bool>> handler, string queue = "", QueueArgument? arguments = null, IObjectSerializer? serializer = null)
        {
            var bytes = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrWhiteSpace(message))
                return;

            var @event = serializer.Deserialize<T>(message);
            if (@event == null)
                return;

            if (handler == null)
                return;

            string key = string.Empty;
            if (e.BasicProperties.TryGetMessageKey(out var msgKey))//修改： 改为使用MessageKey做幂等键 
            {
                //TODO：使用redis进行幂等处理
                Trace.WriteLine($"\tmsg key：{msgKey}", "“消息处理”>");

                key = string.Format(arguments.IdempotenceKeyFormat, $"{queue}:{msgKey}");
                if (await arguments.Redis.ExistsAsync(key))
                {
                    Trace.WriteLine($"幂等检查：\tmsg key：{msgKey}【已处理】忽略", "“消息处理”>");
                    channel.BasicAck(e.DeliveryTag, false); //manua ack.
                    return;
                }
            }

            var policyBuilder = Policy.Handle<Exception>()
            .OrResult<bool>(r => !r);

            IAsyncPolicy<bool> policy = policyBuilder.FallbackAsync(async ct => await Task.FromResult(property.ConfirmedIfException), async ex => await Task.FromResult(property.ConfirmedIfException));

            if (property.RetryTimes > 0)
            {
                var retry = policyBuilder.WaitAndRetryAsync(property.RetryTimes, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
                {
                    var msg = exception.Exception is null
                      ? "业务执行失败"
                      : $"未知错误，{exception.Exception.Message}";
                    Trace.WriteLine($"\t开始第【{retryCount}】次重试 ，原因：{msg}", "“消息处理”>");
                });
                policy = policy.WrapAsync(retry);
            }

            var confirmed = await policy.ExecuteAsync(async () => await handler.Invoke(@event));

            if (confirmed)
            {
                if (!string.IsNullOrEmpty(key))
                    await arguments.Redis.SetAsync(key, 1, arguments.IdempotenceExpires);//redis标记已处理

                channel.BasicAck(e.DeliveryTag, false);
            }
            else
                channel.BasicReject(e.DeliveryTag, true);
        }

        private static Dictionary<string, object> GetArgsByQueueArgument(string routeKey, QueueArgument? arguments)
        {
            if (arguments == null)
                arguments = new QueueArgument();

            var args = new Dictionary<string, object>
                    {
                        { "x-queue-type", arguments.QueueType }
                    };

            if (!string.IsNullOrEmpty(arguments.QueueMode) && !arguments.QueueMode.Equals("default"))
                args.Add("x-queue-mode", arguments.QueueMode);

            if (arguments.QueueType.Equals("quorum"))
            {
                args.Add("x-overflow", "reject-publish");
                args.Add("x-max-length-bytes", 1024 * 1024 * 16);//队列 :quorum ，设置默认最大长度16m，防止消息丢失 将其抛到 dlx galo@2023-5-29 13:22:41
            }

            if (arguments.MessageTtl != 0)
                args.Add("x-message-ttl", arguments.MessageTtl);

            if (arguments.DlxEnabled)
            {
                args.Add("x-dead-letter-exchange", $"{routeKey}.dlx");
                args.Add("x-dead-letter-routing-key", $"{routeKey}.dlk");
            }

            return args;
        }

        /// <summary>
        /// 新增：池化<see cref="IConnection"/> galo@2022-1-16 10:07:32
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        private static IConnection GetOrCreateConnection(IConnectionFactory factory, string queue)
        {
            var key = $"{((ConnectionFactory)factory).Endpoint}:{queue}";

            if (!_pool.TryGetValue(key, out var connection) || connection == null)
            {
                lock (_lock)
                {
                    if (!_pool.TryGetValue(key, out var cnn) || cnn == null)
                    {
                        var @new = factory.CreateConnection();
                        _pool.Add(key, @new);
                        return @new;
                    }
                }
            }
            return _pool[key];
        }

        #endregion

        #region BasicProperties.
        private static PropertyInfo[]? GetMessageKeyProperties(Type type)
        {
            var messageKeyProperties = new[] { type.GetProperty("MessageKey") };
            if (messageKeyProperties is null || !messageKeyProperties.Where(i => i is not null).Any())
            {
                var properties = type.GetProperties();
                if (properties.Any(i => i.HasCustomeAttribute<MessageKeyAttribute>(true)))
                {
                    messageKeyProperties = properties.Where(i => i.HasCustomeAttribute<MessageKeyAttribute>(true)).ToArray();
                }
            }

            return messageKeyProperties;
        }

        private static PropertyInfo[]? GetMessageIdProperties(Type type)
        {
            var messageIdProperties = new[] { type.GetProperty("MessageId") };
            if (messageIdProperties is null || !messageIdProperties.Where(i => i is not null).Any())
            {
                var properties = type.GetProperties();
                if (properties.Any(i => i.HasCustomeAttribute<MessageIdAttribute>(true)))
                {
                    messageIdProperties = properties.Where(i => i.HasCustomeAttribute<MessageIdAttribute>(true)).ToArray();
                }
            }

            return messageIdProperties;
        }

        private static void TrySetBasicProperties<T>(IBasicProperties props, T? msg)
        {
            if (!_basicPropertyMappings.TryGetValue(typeof(T), out var mappings))
                return;

            foreach (var mapping in mappings)
            {
                if (!mappings.TryGetValue(mapping.Key, out var properties))
                    continue;

                props.ClearCustomProperty(mapping.Key);

                var propertiesVal = string.Join("_", properties.Select(i => i.GetValue(msg)));
                if (!string.IsNullOrEmpty(propertiesVal))
                {
                    props.SetCustomProperty(mapping.Key, propertiesVal);
                    if (mapping.Key.Equals("x-message-id"))
                    {
                        //props.ClearMessageId();
                        props.MessageId = propertiesVal;
                    }
                }
            }
        }

        private static void TryCreateOrUpdateBasicPropertyMappings(Type type)
        {
            if (!_basicPropertyMappings.TryGetValue(type, out var mappings))
            {
                var dic = new Dictionary<string, PropertyInfo[]>();

                var messageIdProperties = GetMessageIdProperties(type);
                dic.TryAdd("x-message-id", messageIdProperties.ToArray());

                var messageKeyProperties = GetMessageKeyProperties(type);
                dic.TryAdd("x-message-key", messageKeyProperties.ToArray());

                _basicPropertyMappings.TryAdd(type, dic);
            }
            else
            {
                if (!mappings.TryGetValue("x-message-id", out var properties))
                {
                    mappings.TryAdd("x-message-id", GetMessageIdProperties(type));
                }
                if (!mappings.TryGetValue("x-message-key", out var keyProperties))
                {
                    mappings.TryAdd("x-message-key", GetMessageKeyProperties(type));
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class QueueArgument
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dlxEnabled"></param>
        /// <param name="idempotenceKeyFormat"></param>
        /// <param name="idempotenceExpires"></param>
        /// <param name="msgTtl"></param>
        /// <param name="queueType"></param>
        /// <param name="queueMode"></param>
        /// <param name="declared"></param>
        /// <param name="redis"></param>
        public QueueArgument(
            bool dlxEnabled = false,
            int msgTtl = 0,
            string queueType = "classic",
            string queueMode = "default",
            bool declared = false,
            string? idempotenceKeyFormat = null,
            int idempotenceExpires = 24 * 60 * 3600,
            RedisClient? redis = null)
        {
            DlxEnabled = dlxEnabled;
            MessageTtl = msgTtl;
            QueueType = queueType;
            QueueMode = queueMode;
            ResourceDeclared = declared;
            IdempotenceKeyFormat = idempotenceKeyFormat;
            IdempotenceExpires = idempotenceExpires;
            Redis = redis;

            if (!string.IsNullOrEmpty(idempotenceKeyFormat) && idempotenceKeyFormat.Contains("{0}") && redis == null)
                throw new ArgumentNullException($"{nameof(QueueArgument)}.{nameof(Redis)}");
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DlxEnabled { get; protected set; }

        /// <summary>
        /// classic,quorum,stream
        /// </summary>
        public string QueueType { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public long MessageTtl { get; protected set; }

        /// <summary>
        /// default,lazy
        /// </summary>
        public string QueueMode { get; protected set; }

        /// <summary>
        /// true 创建，false 需要创建
        /// </summary>
        public bool ResourceDeclared { get; protected set; }

        /// <summary>
        /// 幂等Key设置 eg.oc:{queue_name}:{0} {0}占位 QueueName:MessageId
        /// </summary>
        public string IdempotenceKeyFormat { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public RedisClient Redis { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int IdempotenceExpires { get; internal set; }
    }

    /// <summary>
    /// handler property.
    /// </summary>
    public class HandlerProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public HandlerProperty()
        {
            PrefetchCount = 5;
            ConfirmedIfException = false;
            HandlerQty = 1;
            RetryTimes = 0;
            AsyncEnabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefetchCount"></param>
        /// <param name="confirmedIfException"></param>
        /// <param name="retryTimes"></param>
        /// <param name="asyncEnabled"></param>
        public HandlerProperty(ushort prefetchCount, bool confirmedIfException, int retryTimes = 0, bool asyncEnabled = true)
            : this()
        {
            PrefetchCount = prefetchCount;
            ConfirmedIfException = confirmedIfException;
            RetryTimes = retryTimes;
            AsyncEnabled = asyncEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ConfirmedIfException { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort PrefetchCount { get; protected set; }

        /// <summary>
        /// 重试次数 新增：galo@2023-5-29 13:56:17
        /// </summary>
        public int RetryTimes { get; protected set; }

        /// <summary>
        /// 处理器数量 新增：galo@2022-2-9 11:24:36
        /// </summary>
        public int HandlerQty { get; set; }

        /// <summary>
        /// 异步消费者启用 新增：galo@2023-8-23 11:13:10
        /// </summary>
        public bool AsyncEnabled { get; protected set; }
    }

}


namespace RabbitMQ.Client
{
    /// <summary>
    /// 
    /// </summary>
    public static class RabbitMQExtension
    {
        #region BasicProperty:message key.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        public static void ClearMessageKey(this IBasicProperties props)
        {
            ClearCustomProperty(props, "x-message-key");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetMessageKey(this IBasicProperties props, out object? value)
        {
            return TryGetCustomProperty(props, "x-message-key", out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <param name="value"></param>
        public static void SetMessageKey(this IBasicProperties props, object value)
        {
            SetCustomProperty(props, "x-message-key", value);
        }
        #endregion

        #region BasicProperty:CustomProperty.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <param name="key"></param>
        public static void ClearCustomProperty(this IBasicProperties props, string key)
        {
            if (!props.Headers.Any(h => h.Key.Equals(key)))
                return;
            props.Headers.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetCustomProperty(this IBasicProperties props, string key, out object? value)
        {
            if (!props.Headers.Any(h => h.Key.Equals(key)))
            {
                value = null;
                return false;
            }

            value = Encoding.UTF8.GetString((byte[])props.Headers[key]);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetCustomProperty(this IBasicProperties props, string key, object value)
        {
            if (!props.Headers.Any(h => h.Key.Equals(key)))
            {
                props.Headers.Add(key, value);
                return;
            }

            props.Headers[key] = value;
        }
        #endregion
    }
}


namespace RabbitMQ.Client
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConnectionHandlerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public static void RegisterAllHandlers(this IServiceProvider root)
        {
            var classTypes = DependencyContext.Default.GetClassTypes();
            var mis = classTypes.SelectMany(i => i.GetMethods())
            .Where(m => m.GetCustomAttribute<QueueAttribute>(false) != null);
            if (mis is null || !mis.Any())
                return;

            using (var scope = root.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var factory = sp.GetRequiredService<IConnectionFactory>();
                var redis = root.GetRequiredService<RedisClient>();

                foreach (var mi in mis)
                {
                    var queue = mi.GetCustomAttribute<QueueAttribute>();
                    var paras = mi.GetParameters();
                    var first = paras[0];
                    var firstType = first.ParameterType;
                    factory.Handle(new HandlerProperty(20, true, retryTimes: 3), msg =>
                    {
                        using (var scope = root.CreateScope())
                        {
                            var sp = scope.ServiceProvider;
                            var instance = sp.GetRequiredService(mi.DeclaringType);

                            var serializer = sp.GetRequiredService<IObjectSerializer>();

                            #region TODO:优化调用方式 当前为反射
                            var parameters = new List<object>();
                            if (paras.Any())
                            {
                                if (firstType.IsPrimitive || firstType == typeof(string))
                                {
                                    parameters.Add(Convert.ChangeType(msg, firstType));
                                }
                                else
                                {
                                    parameters.Add(msg);
                                }
                            }
                            var rt = mi.Invoke(instance, parameters.ToArray());
                            if (mi.ReturnType.IsAssignableTo(typeof(Task)))
                            {
                                var task = rt as Task;
                                task.ConfigureAwait(false);
                                rt = task.GetType().GetProperty("Result").GetValue(rt);
                            }
                            if (rt is Boolean b)
                                return Task.FromResult(b);

                            return Task.FromResult(true);
                            #endregion
                        }
                    }
                    , msgType: firstType
                    , queue: queue.Name
                    , arguments: new QueueArgument(declared: queue.ResourceDeclared, idempotenceKeyFormat: string.Join(":", queue.Name, "{0}"), redis: redis));
                }
            }
        }
    }
}