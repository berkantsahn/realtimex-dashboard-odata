using Microsoft.AspNetCore.SignalR;
using RealtimeX.Dashboard.Core.Entities;

namespace RealtimeX.Dashboard.Core.Hubs
{
    public class RealTimeHub : Hub
    {
        public async Task SendData(RealTimeData data)
        {
            await Clients.All.SendAsync("ReceiveData", data);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
} 