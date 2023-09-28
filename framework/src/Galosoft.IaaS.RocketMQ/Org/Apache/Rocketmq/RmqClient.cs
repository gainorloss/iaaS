using Galosoft.IaaS.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Org.Apache.Rocketmq
{
    /// <summary>
    /// 
    /// </summary>
    public class RmqClient : IDisposable
    {
        private ClientConfig _clientConfig;
        private readonly IObjectSerializer _objectSerializer;
        private static IDictionary<string, Producer> Producers = new ConcurrentDictionary<string, Producer>();
        private static IDictionary<string, SimpleConsumer> SimpleConsumers = new ConcurrentDictionary<string, SimpleConsumer>();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="objectSerializer"></param>
        /// <param name="clientConfig"></param>
        public RmqClient(IObjectSerializer objectSerializer, ClientConfig clientConfig)
        {
            _objectSerializer = objectSerializer;
            _clientConfig = clientConfig;
        }

        /// <summary>
        /// distribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="prop"></param>
        /// <param name="topic"></param>
        /// <param name="tag"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task DistributeAsync<T>(T msg, RmqProperty prop, string topic, string tag = "*", params string[] keys)
        {
            if (string.IsNullOrWhiteSpace(tag))
                tag = "*";

            var producer = await GetOrCreateProducerAsync(topic);
            var message = new Message.Builder()
                  .SetTag(tag)
                  .SetTopic(topic)
                  .SetKeys(keys)
                  .SetBody(Encoding.UTF8.GetBytes(_objectSerializer.Serialize(msg)));

            if (prop.Delay > 0)
                message.SetDeliveryTimestamp(DateTime.Now + TimeSpan.FromMilliseconds(prop.Delay));

            if (!string.IsNullOrEmpty(prop.MessageGroup))
                message.SetMessageGroup(prop.MessageGroup);

            await producer.Send(message.Build());
        }

        /// <summary>
        /// handle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="topic"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task HandleAsync<T>(Func<T, Task<bool>> handler, string topic, string tag = "*")
        {
            if (string.IsNullOrWhiteSpace(tag))
                tag = "*";

            var consumer = await GetOrCreateSimpleConsumer(topic, tag);
            Tracer.Trace($"=======正在订阅 TOPIC:{topic},TAG:{tag}======", "rmq 处理器");
            await Task.Run(async () =>
            {
                var idx = 0;
                while (true)
                {
                    ++idx;
                    try
                    {
                        var mvs = await consumer.Receive(16, TimeSpan.FromSeconds(15));
                        if (!mvs.Any())
                        {
                            Tracer.Trace($"{idx}【{Thread.CurrentThread.ManagedThreadId}】:no msg......", "rmq 处理器");
                            Thread.Sleep(1000);
                            continue;
                        }

                        foreach (var mv in mvs)
                        {
                            var body = Encoding.UTF8.GetString(mv.Body);
                            var msg = _objectSerializer.Deserialize<T>(body);
                            var rt = await handler?.Invoke(msg);
                            if (!rt)
                            {
                                Tracer.Trace($"{idx}【{Thread.CurrentThread.ManagedThreadId}】:{mv.MessageId}", "rmq 处理器");
                                continue;
                            }
                            await consumer.Ack(mv);
                            Tracer.Trace($"{idx}【{Thread.CurrentThread.ManagedThreadId}】:{mv.MessageId} 已 ACK!!!", "rmq 处理器");
                        }
                    }
                    catch (Exception e)
                    {
                        Tracer.Trace($"{idx}【{Thread.CurrentThread.ManagedThreadId}】:no msg......[消息拉取异常][{e.Message}]", "rmq 处理器");
                        await Task.Delay(5000);
                    }
                }
            });
        }

        private string GetConsumerGroup(string topic, string tag)
        {
            var cGroup = topic;
            if (!string.IsNullOrEmpty(tag) && !tag.Equals("*"))
                cGroup = $"{topic}-{tag}";
            return cGroup;
        }

        private async Task<SimpleConsumer> GetOrCreateSimpleConsumer(string topic, string tag)
        {
            var cGroup = GetConsumerGroup(topic, tag);

            if (SimpleConsumers.TryGetValue(cGroup, out var consumer) && consumer is not null)
                return consumer;

            consumer = await new SimpleConsumer.Builder()
                 .SetClientConfig(_clientConfig)
                 .SetAwaitDuration(TimeSpan.FromSeconds(5))
                 .SetConsumerGroup(cGroup)
                 .SetSubscriptionExpression(new Dictionary<string, FilterExpression> { { topic, new FilterExpression(tag) } })
                 .Build();
            SimpleConsumers[topic] = consumer;

            return consumer;
        }

        private async Task<Producer> GetOrCreateProducerAsync(string topic)
        {
            if (Producers.TryGetValue(topic, out var producer) && producer is not null)
                return producer;

            producer = await new Producer.Builder()
                .SetClientConfig(_clientConfig)
                .SetTopics(topic)
                .Build();
            Producers[topic] = producer;

            return producer;
        }

        public void Dispose()
        {
            foreach (var producer in Producers)
            {
                if (producer.Value is null)
                    continue;
                producer.Value.Dispose();
            }
            foreach (var consumer in SimpleConsumers)
            {
                if (consumer.Value is null)
                    continue;
                consumer.Value.Dispose();
            }
        }
    }
    public class RmqProperty
    {
        public RmqProperty(int delay = 0, string mGroup = null)
        {
            Delay = delay;
            MessageGroup = mGroup;

        }
        public int Delay { get; protected set; }
        public string MessageGroup { get; protected set; }
    }
}
