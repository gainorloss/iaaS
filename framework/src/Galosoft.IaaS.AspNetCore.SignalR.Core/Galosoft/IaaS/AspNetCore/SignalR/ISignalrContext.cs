namespace Galosoft.IaaS.AspNetCore.SignalR
{
    public interface ISignalrContext<T>
    {
        string InstanceId { get; }
        T Message { get; }
    }
}
