using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;
using RealtimeX.Dashboard.Core.Hubs;

namespace RealtimeX.Dashboard.Services
{
    public class RealTimeDataService : IRealTimeDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediaService _mediaService;
        private readonly IHubContext<RealTimeHub> _hubContext;

        public RealTimeDataService(IUnitOfWork unitOfWork, IMediaService mediaService, IHubContext<RealTimeHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mediaService = mediaService;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<RealTimeData>> GetAllDataAsync()
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            return await repository.GetAllAsync();
        }

        public async Task<RealTimeData> GetDataByIdAsync(string id)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            return await repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<RealTimeData>> GetDataByDeviceIdAsync(string deviceId)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            return await repository.FindAsync(d => d.DeviceId == deviceId);
        }

        public async Task<IEnumerable<RealTimeData>> GetDataByTypeAsync(string dataType)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            return await repository.FindAsync(d => d.DataType == dataType);
        }

        public async Task<RealTimeData> AddDataAsync(RealTimeData data)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            await repository.AddAsync(data);
            await _unitOfWork.SaveChangesAsync();

            // Send real-time update through SignalR
            await SendDataAsync(data);

            return data;
        }

        public async Task<bool> UpdateDataAsync(string id, RealTimeData data)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            var existingData = await repository.GetByIdAsync(id);
            if (existingData == null)
                return false;

            existingData.DeviceId = data.DeviceId;
            existingData.DataType = data.DataType;
            existingData.Value = data.Value;
            existingData.Timestamp = data.Timestamp;

            await repository.UpdateAsync(existingData);
            await _unitOfWork.SaveChangesAsync();

            // Notify clients about the updated data
            await _hubContext.Clients.All.SendAsync("UpdateData", existingData);

            return true;
        }

        public async Task<bool> DeleteDataAsync(string id)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            var data = await repository.GetByIdAsync(id);
            if (data == null)
                return false;

            await repository.DeleteAsync(data);
            await _unitOfWork.SaveChangesAsync();

            // Notify clients about the deleted data
            await _hubContext.Clients.All.SendAsync("DeleteData", id);

            return true;
        }

        public async Task<IEnumerable<RealTimeData>> GetLatestDataAsync(string deviceId, int count)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            var data = await repository.FindAsync(d => d.DeviceId == deviceId);
            return data.OrderByDescending(d => d.Timestamp).Take(count);
        }

        public async Task<IEnumerable<RealTimeData>> GetDataByDateRangeAsync(string deviceId, DateTime startDate, DateTime endDate)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            var data = await repository.FindAsync(d => 
                d.DeviceId == deviceId && 
                d.Timestamp >= startDate && 
                d.Timestamp <= endDate);
            return data.OrderBy(d => d.Timestamp);
        }

        public async Task ProcessDataAsync(int dataId)
        {
            var repository = _unitOfWork.GetRepository<RealTimeData>();
            var data = await repository.GetByIdAsync(dataId.ToString());
            if (data != null)
            {
                data.IsProcessed = true;
                data.ProcessedAt = DateTime.UtcNow;
                await repository.UpdateAsync(data);
                await _unitOfWork.SaveChangesAsync();

                // Send real-time update through SignalR
                await SendDataAsync(data);
            }
        }

        public async Task SendDataAsync(RealTimeData data)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveRealTimeData", data);
        }
    }
} 