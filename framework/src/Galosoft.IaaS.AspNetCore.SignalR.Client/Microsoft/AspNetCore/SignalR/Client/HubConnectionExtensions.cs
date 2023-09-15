using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SignalR.Client
{
    public static class HubConnectionExtensions
    {
        public static void OnNotifyAsync<T>(this HubConnection myConn, Action<T> handler)
        {
            if (handler == null)
                return;

            myConn.On<T>("NotifyAsync", handler);
        }

        public static void OnReceiveAsync<T>(this HubConnection myConn, Action<T> handler)
        {
            if (handler == null)
                return;

            myConn.On<T>("ReceiveAsync", handler);
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myConn"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task BroadcastAsync<T>(this HubConnection myConn, T msg)
        {
            await myConn.InvokeAsync("broadcastAsync", msg);
        }

        /// <summary>
        /// 广播给其他客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myConn"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task BroadcastOthersAsync<T>(this HubConnection myConn, T msg)
        {
            await myConn.InvokeAsync("broadcastOthersAsync", msg);
        }

        /// <summary>
        /// 转发给当前客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myConn"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task UnicastMeAsync<T>(this HubConnection myConn, T msg)
        {
            await myConn.InvokeAsync("unicastMeAsync", msg);
        }

        public static async Task UnicastAsync<T>(this HubConnection myConn, T msg, params string[] users)
        {
            await myConn.InvokeAsync("unicastAsync", msg, users);
        }

        public static async Task MulticastAsync<T>(this HubConnection myConn, T msg, params string[] groups)
        {
            await myConn.InvokeAsync("multicastAsync", msg);
        }

        public static async Task MulticastOthersAsync<T>(this HubConnection myConn, T msg, string group)
        {
            await myConn.InvokeAsync("multicastOthersAsync", msg);
        }
    }
}
