using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealtimeX.Dashboard.Core.Entities
{
    public class Announcement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("audioUrl")]
        public string AudioUrl { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("modifiedAt")]
        public DateTime? ModifiedAt { get; set; }

        [BsonElement("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [BsonElement("createdBy")]
        public string CreatedBy { get; set; }

        [BsonElement("updatedBy")]
        public string UpdatedBy { get; set; }

        [BsonElement("recipients")]
        public List<string> Recipients { get; set; } = new List<string>();

        [BsonElement("readBy")]
        public List<string> ReadBy { get; set; } = new List<string>();
    }
} 