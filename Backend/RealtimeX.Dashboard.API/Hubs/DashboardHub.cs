using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.API.Hubs
{
    public class DashboardHub : Hub, ISignalRHub
    {
        private readonly IRealTimeDataService _realTimeDataService;
        private readonly IAnnouncementService _announcementService;

        public DashboardHub(
            IRealTimeDataService realTimeDataService,
            IAnnouncementService announcementService)
        {
            _realTimeDataService = realTimeDataService;
            _announcementService = announcementService;
        }

        public async Task SendRealTimeData(RealTimeData data)
        {
            var savedData = await _realTimeDataService.AddDataAsync(data);
            await Clients.Group(data.DeviceId).SendAsync("ReceiveRealTimeData", savedData);
        }

        public async Task SendAnnouncement(Announcement announcement)
        {
            var savedAnnouncement = await _announcementService.CreateAnnouncementAsync(announcement);
            await Clients.All.SendAsync("ReceiveAnnouncement", savedAnnouncement);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
} 