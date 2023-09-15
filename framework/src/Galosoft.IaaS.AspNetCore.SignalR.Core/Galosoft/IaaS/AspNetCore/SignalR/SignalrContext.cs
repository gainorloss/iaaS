using System;

namespace Galosoft.IaaS.AspNetCore.SignalR
{
    public class SignalrContext<T>
        : ISignalrContext<T>
    {
        public SignalrContext()
        {
            OccuredOn = DateTime.Now;
        }

        public SignalrContext(
            string instanceId,
            string connectionId)
            :this()
        {
            InstanceId = instanceId;
            ConnectionId = connectionId;
        }

        public string InstanceId { get; set; }

        public string ConnectionId { get; set; }

        public T Message { get; set; }

        public DateTime OccuredOn { get; set; }

        public SignalrContext<T> SetMessage(T msg)
        {
            Message = msg;
            return this;
        }
    }
}
