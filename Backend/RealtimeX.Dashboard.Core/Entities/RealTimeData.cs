using System;

namespace RealtimeX.Dashboard.Core.Entities
{
    public class RealTimeData
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsProcessed { get; set; }
    }
} 