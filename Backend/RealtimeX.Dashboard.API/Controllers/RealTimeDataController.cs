using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.API.Controllers
{
    [Authorize]
    public class RealTimeDataController : ODataController
    {
        private readonly IRealTimeDataService _realTimeDataService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RealTimeData> _repository;

        public RealTimeDataController(IRealTimeDataService realTimeDataService, IUnitOfWork unitOfWork)
        {
            _realTimeDataService = realTimeDataService;
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<RealTimeData>();
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var data = await _repository.GetAllAsync();
            return Ok(data);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] string key)
        {
            var data = await _repository.GetByIdAsync(key);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        public async Task<IActionResult> Post([FromBody] RealTimeData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.AddAsync(data);
            await _unitOfWork.SaveChangesAsync();

            // SignalR ile gerçek zamanlı güncelleme gönder
            await _realTimeDataService.SendDataAsync(data);

            return Created(data);
        }

        public async Task<IActionResult> Put([FromRoute] string key, [FromBody] RealTimeData update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await _repository.GetByIdAsync(key);
            if (data == null)
            {
                return NotFound();
            }

            // Entity güncelleme
            await _repository.UpdateAsync(update);
            await _unitOfWork.SaveChangesAsync();

            // SignalR ile gerçek zamanlı güncelleme gönder
            await _realTimeDataService.SendDataAsync(update);

            return Updated(update);
        }

        public async Task<IActionResult> Delete([FromRoute] string key)
        {
            var data = await _repository.GetByIdAsync(key);
            if (data == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(data);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
} 