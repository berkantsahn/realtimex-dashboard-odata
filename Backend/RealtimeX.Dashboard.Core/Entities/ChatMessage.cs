using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealtimeX.Dashboard.Core.Entities
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("senderId")]
        public string SenderId { get; set; }

        [BsonElement("receiverId")]
        public string ReceiverId { get; set; }

        [BsonElement("chatId")]
        public string ChatId { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("sentAt")]
        public DateTime SentAt { get; set; }

        [BsonElement("isRead")]
        public bool IsRead { get; set; }

        [BsonElement("readAt")]
        public DateTime? ReadAt { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = "text";

        [BsonElement("mediaUrl")]
        public string? MediaUrl { get; set; }

        [BsonElement("mediaThumbnailUrl")]
        public string? MediaThumbnailUrl { get; set; }

        [BsonElement("mediaSize")]
        public long? MediaSize { get; set; }

        [BsonElement("mediaName")]
        public string? MediaName { get; set; }

        [BsonElement("mediaMimeType")]
        public string? MediaMimeType { get; set; }

        [BsonElement("mediaDuration")]
        public TimeSpan? MediaDuration { get; set; }
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