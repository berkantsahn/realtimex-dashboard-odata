using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnnouncementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Announcement>> GetActiveAnnouncementsAsync()
        {
            var announcements = await _unitOfWork.Repository<Announcement>()
                .FindAsync(a => a.IsActive && 
                               (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow));

            return announcements.OrderByDescending(a => a.CreatedAt);
        }

        public async Task<Announcement> CreateAnnouncementAsync(Announcement announcement)
        {
            announcement.CreatedAt = DateTime.UtcNow;
            announcement.IsActive = true;

            await _unitOfWork.Repository<Announcement>().AddAsync(announcement);
            await _unitOfWork.CompleteAsync();

            return announcement;
        }

        public async Task<bool> UpdateAnnouncementAsync(Announcement announcement)
        {
            var existingAnnouncement = await _unitOfWork.Repository<Announcement>()
                .GetByIdAsync(announcement.Id);

            if (existingAnnouncement == null)
            {
                return false;
            }

            existingAnnouncement.Title = announcement.Title;
            existingAnnouncement.Message = announcement.Message;
            existingAnnouncement.AudioUrl = announcement.AudioUrl;
            existingAnnouncement.ExpiresAt = announcement.ExpiresAt;
            existingAnnouncement.IsActive = announcement.IsActive;

            await _unitOfWork.Repository<Announcement>().UpdateAsync(existingAnnouncement);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteAnnouncementAsync(int id)
        {
            var announcement = await _unitOfWork.Repository<Announcement>().GetByIdAsync(id);
            if (announcement == null)
            {
                return false;
            }

            await _unitOfWork.Repository<Announcement>().DeleteAsync(announcement);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<Announcement> GetAnnouncementByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Announcement>().GetByIdAsync(id);
        }
    }
} 