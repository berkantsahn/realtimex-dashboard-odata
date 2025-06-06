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

        public RealTimeDataController(IRealTimeDataService realTimeDataService, IUnitOfWork unitOfWork)
        {
            _realTimeDataService = realTimeDataService;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var data = _unitOfWork.Repository<RealTimeData>().GetAll();
            return Ok(data);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] int key)
        {
            var data = await _unitOfWork.Repository<RealTimeData>().GetByIdAsync(key);
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

            await _unitOfWork.Repository<RealTimeData>().AddAsync(data);
            await _unitOfWork.CompleteAsync();

            // SignalR ile gerçek zamanlı güncelleme gönder
            await _realTimeDataService.BroadcastDataAsync(data);

            return Created(data);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] RealTimeData update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await _unitOfWork.Repository<RealTimeData>().GetByIdAsync(key);
            if (data == null)
            {
                return NotFound();
            }

            // Entity güncelleme
            _unitOfWork.Repository<RealTimeData>().Update(update);
            await _unitOfWork.CompleteAsync();

            // SignalR ile gerçek zamanlı güncelleme gönder
            await _realTimeDataService.BroadcastDataAsync(update);

            return Updated(update);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var data = await _unitOfWork.Repository<RealTimeData>().GetByIdAsync(key);
            if (data == null)
            {
                return NotFound();
            }

            _unitOfWork.Repository<RealTimeData>().Delete(data);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
} 