using System;

namespace RealtimeX.Dashboard.Core.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public MediaType Type { get; set; }
        public DateTime SentAt { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string MediaThumbnailUrl { get; set; }
        public long? MediaSize { get; set; }
        public string MediaName { get; set; }
        public string MediaMimeType { get; set; }
        public int? MediaDuration { get; set; } // Ses/Video süresi (saniye)
    }

    public enum MediaType
    {
        Text,
        Audio,
        Image,
        Video,
        Document
    }
} 