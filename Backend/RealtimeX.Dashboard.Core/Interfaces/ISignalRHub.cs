using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface ISignalRHub
    {
        Task SendRealTimeData(RealTimeData data);
        Task SendAnnouncement(Announcement announcement);
        Task JoinGroup(string groupName);
        Task LeaveGroup(string groupName);
    }
} 