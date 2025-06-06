using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;
using RealtimeX.Dashboard.Core.Hubs;

namespace RealtimeX.Dashboard.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediaService _mediaService;
        private readonly IHubContext<DashboardHub> _hubContext;

        public AnnouncementService(IUnitOfWork unitOfWork, IMediaService mediaService, IHubContext<DashboardHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mediaService = mediaService;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<Announcement>> GetAllAnnouncementsAsync()
        {
            var repository = _unitOfWork.GetRepository<Announcement>();
            return await repository.GetAllAsync();
        }

        public async Task<Announcement> GetAnnouncementByIdAsync(string id)
        {
            var repository = _unitOfWork.GetRepository<Announcement>();
            return await repository.GetByIdAsync(id);
        }

        public async Task<Announcement> AddAnnouncementAsync(Announcement announcement)
        {
            var repository = _unitOfWork.GetRepository<Announcement>();
            announcement.CreatedAt = DateTime.UtcNow;
            await repository.AddAsync(announcement);
            await _unitOfWork.SaveChangesAsync();
            return announcement;
        }

        public async Task<IEnumerable<Announcement>> GetAnnouncementsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var repository = _unitOfWork.GetRepository<Announcement>();
            return await repository.FindAsync(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate);
        }

        public async Task<bool> UpdateAnnouncementAsync(string id, Announcement announcement)
        {
            var repository = _unitOfWork.GetRepository<Announcement>();
            var existingAnnouncement = await repository.GetByIdAsync(id);
            if (existingAnnouncement == null)
                return false;

            existingAnnouncement.Title = announcement.Title;
            existingAnnouncement.Message = announcement.Message;
            existingAnnouncement.AudioUrl = announcement.AudioUrl;
            existingAnnouncement.IsActive = announcement.IsActive;
            existingAnnouncement.UpdatedAt = DateTime.UtcNow;
            existingAnnouncement.ModifiedAt = DateTime.UtcNow;
            existingAnnouncement.ExpiryDate = announcement.ExpiryDate;
            existingAnnouncement.UpdatedBy = announcement.UpdatedBy;
            existingAnnouncement.Recipients = announcement.Recipients;

            await repository.UpdateAsync(existingAnnouncement);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAnnouncementAsync(string id)
        {
            var repository = _unitOfWork.GetRepository<Announcement>();
            var announcement = await repository.GetByIdAsync(id);
            if (announcement == null)
                return false;

            await repository.DeleteAsync(announcement);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Announcement>> GetActiveAnnouncementsAsync()
        {
            var repository = _unitOfWork.GetRepository<Announcement>();
            var now = DateTime.UtcNow;
            return await repository.FindAsync(a => 
                a.IsActive && 
                (!a.ExpiryDate.HasValue || a.ExpiryDate > now));
        }

        public async Task<Announcement> CreateAnnouncementAsync(Announcement announcement)
        {
            announcement.CreatedAt = DateTime.UtcNow;
            announcement.IsActive = true;
            
            var repository = _unitOfWork.GetRepository<Announcement>();
            await repository.AddAsync(announcement);
            await _unitOfWork.SaveChangesAsync();

            return announcement;
        }

        public async Task SendAnnouncementAsync(Announcement announcement)
        {
            if (announcement.Recipients != null && announcement.Recipients.Count > 0)
            {
                foreach (var recipientId in announcement.Recipients)
                {
                    await _hubContext.Clients.User(recipientId)
                        .SendAsync("ReceiveAnnouncement", announcement);
                }
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement", announcement);
            }
        }
    }
} 