using Dev.ConsoleApp.Interceptors;
using Galosoft.IaaS.RabbitMQ;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dev.ConsoleApp.Handlers
{

    [RT]
    internal class SpringBootAmqpTestRequestHandler
    {
        [RabbitMQHandler("spring.boot.amqp.test", declared: true)]
        public async Task<bool> Handle(SpringBootAmqpTestRequest e)
        {
            Trace.WriteLine($"\t于{DateTime.Now.ToLongTimeString()}\t接收到{e.Message}", "“dlx handler”>");
            return true;
        }
    }
}
