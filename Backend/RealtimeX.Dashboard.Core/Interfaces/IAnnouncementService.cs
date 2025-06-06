using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface IAnnouncementService
    {
        Task<IEnumerable<Announcement>> GetAllAnnouncementsAsync();
        Task<Announcement> GetAnnouncementByIdAsync(string id);
        Task<Announcement> AddAnnouncementAsync(Announcement announcement);
        Task<IEnumerable<Announcement>> GetAnnouncementsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> UpdateAnnouncementAsync(string id, Announcement announcement);
        Task<bool> DeleteAnnouncementAsync(string id);
        Task<IEnumerable<Announcement>> GetActiveAnnouncementsAsync();
        Task<Announcement> CreateAnnouncementAsync(Announcement announcement);
        Task SendAnnouncementAsync(Announcement announcement);
    }
} 