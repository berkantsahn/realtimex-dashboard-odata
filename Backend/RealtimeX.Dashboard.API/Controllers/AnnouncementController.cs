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
    public class AnnouncementController : ODataController
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Announcement> _repository;

        public AnnouncementController(IAnnouncementService announcementService, IUnitOfWork unitOfWork)
        {
            _announcementService = announcementService;
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<Announcement>();
        }

        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var announcements = await _repository.GetAllAsync();
            return Ok(announcements);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] string key)
        {
            var announcement = await _repository.GetByIdAsync(key);
            if (announcement == null)
            {
                return NotFound();
            }
            return Ok(announcement);
        }

        public async Task<IActionResult> Post([FromBody] Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.AddAsync(announcement);
            await _unitOfWork.SaveChangesAsync();

            // SignalR ile gerçek zamanlı güncelleme gönder
            await _announcementService.SendAnnouncementAsync(announcement);

            return Created(announcement);
        }

        public async Task<IActionResult> Put([FromRoute] string key, [FromBody] Announcement update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var announcement = await _repository.GetByIdAsync(key);
            if (announcement == null)
            {
                return NotFound();
            }

            // Entity güncelleme
            await _repository.UpdateAsync(update);
            await _unitOfWork.SaveChangesAsync();

            // SignalR ile gerçek zamanlı güncelleme gönder
            await _announcementService.SendAnnouncementAsync(update);

            return Updated(update);
        }

        public async Task<IActionResult> Delete([FromRoute] string key)
        {
            var announcement = await _repository.GetByIdAsync(key);
            if (announcement == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(announcement);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
} 