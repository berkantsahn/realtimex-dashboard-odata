using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface IRealTimeDataService
    {
        Task<IEnumerable<RealTimeData>> GetLatestDataAsync(string deviceId, int count);
        Task<RealTimeData> AddDataAsync(RealTimeData data);
        Task<IEnumerable<RealTimeData>> GetDataByDateRangeAsync(string deviceId, DateTime startDate, DateTime endDate);
        Task ProcessDataAsync(int dataId);
    }
} 