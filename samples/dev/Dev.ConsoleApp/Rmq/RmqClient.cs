using Galosoft.IaaS.Core;
using Org.Apache.Rocketmq;
using RabbitMQ.Client;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Dev.ConsoleApp.Rmq
{
    internal class RmqClient : IDisposable
    {
        private ClientConfig clientConfig;
        private readonly IObjectSerializer _objectSerializer;
        Producer _producer;
        public RmqClient(IObjectSerializer objectSerializer, IConfiguration configuration)
        {
            _objectSerializer = objectSerializer;
            if (clientConfig is null)
            {
                var configBuilder = new ClientConfig.Builder();

                var endpoints = configuration.GetValue<string>("RocketMQ:Endpoints");
                if (!string.IsNullOrEmpty(endpoints))
                    configBuilder.SetEndpoints(endpoints);

                var ssl = configuration.GetValue<bool>("RocketMQ:Ssl");
                configBuilder.EnableSsl(ssl);

                clientConfig = configBuilder.Build();
            }
        }

        public async Task DistributeAsync<T>(T msg, string topic, RmqProperty property, params string[] keys)
        {
            Tracer.Trace($"hashcode:{GetHashCode()}", $"分发");
            await InitializeAsync();
            var message = new Message.Builder()
                  .SetTopic(topic)
                  .SetKeys(keys)
                  .SetBody(Encoding.UTF8.GetBytes(_objectSerializer.Serialize(msg)));
            if (property.Delay > 0)
                message.SetDeliveryTimestamp(DateTime.Now + TimeSpan.FromMilliseconds(property.Delay));

            await _producer.Send(message.Build());
        }

        public async Task HandleAsync<T>(Func<T, Task<bool>> handler, string topic, string consumerGroup)
        {
            var consumer = await new SimpleConsumer.Builder()
                 .SetClientConfig(clientConfig)
                 .SetAwaitDuration(TimeSpan.FromSeconds(5))
                 .SetConsumerGroup(consumerGroup)
                 .SetSubscriptionExpression(new Dictionary<string, FilterExpression> { { topic, new FilterExpression("*") } })
                 .Build();
            Tracer.Trace($"======={consumerGroup}正在订阅{topic}======", "rmq 处理器");
            var idx = 0;
            while (true)
            {
                ++idx;
                //Tracer.Trace($"{++idx}:尝试获取", "rmq 处理器");
                var mvs = await consumer.Receive(16, TimeSpan.FromSeconds(15));
                if (!mvs.Any())
                {
                    Tracer.Trace($"{idx}:no msg......", "rmq 处理器");
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
                        Tracer.Trace($"{idx}:{mv.MessageId}:{body}", "rmq 处理器");
                        continue;
                    }
                    await consumer.Ack(mv);
                    Tracer.Trace($"{idx}:{mv.MessageId}:{Encoding.UTF8.GetString(mv.Body)} 已ack!!!", "rmq 处理器");
                }
            }
        }

        public void Dispose()
        {
            _producer.Dispose();
        }

        private async Task InitializeAsync()
        {
            if (_producer is not null)
                return;

            _producer = await new Producer.Builder()
                .SetClientConfig(clientConfig)
                .Build();
        }
    }
    public class RmqProperty
    {
        public RmqProperty(int delay = 0)
        {
            Delay = delay;
        }
        public int Delay { get; protected set; }
    }
}
