using System.Threading.Tasks;

namespace Galosoft.IaaS.AspNetCore.SignalR
{
    public interface ISignalrClient
    {
        /// <summary>
        /// 通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task NotifyAsync<T>(T msg);


        /// <summary>
        /// 接收
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task ReceiveAsync<T>(T msg);
    }
}
