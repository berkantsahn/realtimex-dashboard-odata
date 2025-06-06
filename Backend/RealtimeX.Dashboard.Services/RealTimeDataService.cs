using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.Services
{
    public class RealTimeDataService : IRealTimeDataService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RealTimeDataService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RealTimeData>> GetLatestDataAsync(string deviceId, int count)
        {
            var data = await _unitOfWork.Repository<RealTimeData>()
                .FindAsync(d => d.DeviceId == deviceId);

            return data.OrderByDescending(d => d.Timestamp)
                      .Take(count);
        }

        public async Task<RealTimeData> AddDataAsync(RealTimeData data)
        {
            data.Timestamp = DateTime.UtcNow;
            data.IsProcessed = false;

            await _unitOfWork.Repository<RealTimeData>().AddAsync(data);
            await _unitOfWork.CompleteAsync();

            return data;
        }

        public async Task<IEnumerable<RealTimeData>> GetDataByDateRangeAsync(string deviceId, DateTime startDate, DateTime endDate)
        {
            var data = await _unitOfWork.Repository<RealTimeData>()
                .FindAsync(d => d.DeviceId == deviceId && 
                               d.Timestamp >= startDate && 
                               d.Timestamp <= endDate);

            return data.OrderBy(d => d.Timestamp);
        }

        public async Task ProcessDataAsync(int dataId)
        {
            var data = await _unitOfWork.Repository<RealTimeData>().GetByIdAsync(dataId);
            if (data != null)
            {
                data.IsProcessed = true;
                await _unitOfWork.Repository<RealTimeData>().UpdateAsync(data);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
} 