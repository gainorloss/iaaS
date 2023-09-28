using RabbitMQ.Client;
using System;
using System.ComponentModel.DataAnnotations;

namespace Galosoft.IaaS.RabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class QueueAttribute : Attribute
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name"></param>
        public QueueAttribute(string name, bool declared = false)
        {
            Name = name;
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
    }
}
