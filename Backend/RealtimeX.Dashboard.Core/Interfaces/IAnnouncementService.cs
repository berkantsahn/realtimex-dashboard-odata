using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface IAnnouncementService
    {
        Task<IEnumerable<Announcement>> GetActiveAnnouncementsAsync();
        Task<Announcement> CreateAnnouncementAsync(Announcement announcement);
        Task<bool> UpdateAnnouncementAsync(Announcement announcement);
        Task<bool> DeleteAnnouncementAsync(int id);
        Task<Announcement> GetAnnouncementByIdAsync(int id);
    }
} 