using RabbitMQ.Client;
using System;
using System.ComponentModel.DataAnnotations;

namespace Galosoft.IaaS.RabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RabbitMQHandlerAttribute : Attribute, IHandlerProperty
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefetchCount"></param>
        /// <param name="confirmedIfException"></param>
        /// <param name="retryTimes"></param>
        /// <param name="asyncEnabled"></param>
        /// <param name="declared"></param>
        public RabbitMQHandlerAttribute(
            string name,
            ushort prefetchCount = 20,
            bool confirmedIfException = true,
            int retryTimes = 3,
            bool asyncEnabled = true,
            bool declared = false)
        {
            Name = name;
            PrefetchCount = prefetchCount;
            ConfirmedIfException = confirmedIfException;
            RetryTimes = retryTimes;
            AsyncEnabled = asyncEnabled;
            ResourceDeclared = declared;
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Name { get; protected set; }

        /// <summary>
        /// true 创建，false 需要创建
        /// </summary>
        public bool ResourceDeclared { get; protected set; }

        public bool ConfirmedIfException { get; protected set; }

        public ushort PrefetchCount { get; protected set; }

        public int RetryTimes { get; protected set; }

        public bool AsyncEnabled { get; protected set; }
    }
}
