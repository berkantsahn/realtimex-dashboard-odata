using System;

namespace RealtimeX.Dashboard.Core.Entities
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string AudioUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
    }
} 