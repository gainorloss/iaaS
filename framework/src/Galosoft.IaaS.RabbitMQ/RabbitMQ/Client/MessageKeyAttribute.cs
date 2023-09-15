using System;

namespace RabbitMQ.Client
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MessageKeyAttribute
        : Attribute
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="order"></param>
        public MessageKeyAttribute(int order = 0)
        {
            Order = order;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Order { get; }
    }
}
