using Galosoft.IaaS.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SignalR
{
    public abstract class NotifyHubBase
     : Hub<ISignalrClient>
    {
        /// <summary>
        /// default group
        /// </summary>
        protected string DefaultGroup = "default";

        /// <summary>
        /// Hub connection id.
        /// </summary>
        protected string InstanceId = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 通知 全局通知 例如 频道加入、连接、断连、重连等
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual async Task NotifyAsync(object msg)
        {
            await Clients.All.NotifyAsync(SignalrContext<object>().SetMessage(msg));
        }

        public virtual async Task BroadcastAsync(object msg)
        {
            await Clients.All.ReceiveAsync(SignalrContext<object>().SetMessage(msg));
        }

        public virtual async Task BroadcastOthersAsync(object msg)
        {
            await Clients.Others.ReceiveAsync(SignalrContext<object>().SetMessage(msg));
        }

        public virtual async Task UnicastMeAsync(object msg)
        {
            await Clients.Caller.ReceiveAsync(SignalrContext<object>().SetMessage(msg));
        }

        public virtual async Task UnicastAsync(object msg, params string[] users)
        {
            await Clients.Users(users).ReceiveAsync(SignalrContext<object>().SetMessage(msg));
        }

        public virtual async Task MulticastAsync(object msg, params string[] groups)
        {
            await Clients.Groups(groups).ReceiveAsync(SignalrContext<object>().SetMessage(msg));
        }

        public virtual async Task MulticastOthersAsync(object msg, string group)
        {
            await Clients.OthersInGroup(group).ReceiveAsync(SignalrContext<object>().SetMessage(msg));
        }

        public override async Task OnConnectedAsync()
        {
            await AddToGroupAsync(DefaultGroup);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromGroupAsync(DefaultGroup);
            await base.OnDisconnectedAsync(exception);
        }

        protected async Task AddToGroupAsync(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await NotifyAsync($"Global：{Context.ConnectionId}连接，加入频道：{DefaultGroup}");
        }

        protected async Task RemoveFromGroupAsync(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await NotifyAsync($"Global：{Context.ConnectionId}离开频道：{DefaultGroup}，断开连接");
        }

        protected SignalrContext<T> SignalrContext<T>()
        {
            return new SignalrContext<T>(InstanceId, Context.ConnectionId);
        }
    }
}
