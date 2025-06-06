using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealtimeX.Dashboard.Core.Entities
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public List<string> Participants { get; set; } = new List<string>();
        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string GroupImage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}