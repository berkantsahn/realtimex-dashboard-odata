using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealtimeX.Dashboard.Core.Entities
{
    public class RealTimeData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("deviceId")]
        public string DeviceId { get; set; }

        [BsonElement("dataType")]
        public string DataType { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("isProcessed")]
        public bool IsProcessed { get; set; }

        [BsonElement("processedAt")]
        public DateTime? ProcessedAt { get; set; }
    }
} 