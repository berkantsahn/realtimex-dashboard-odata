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
    public class AnnouncementsController : ODataController
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IUnitOfWork _unitOfWork;

        public AnnouncementsController(IAnnouncementService announcementService, IUnitOfWork unitOfWork)
        {
            _announcementService = announcementService;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var announcements = _unitOfWork.Repository<Announcement>().GetAll();
            return Ok(announcements);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] int key)
        {
            var announcement = await _unitOfWork.Repository<Announcement>().GetByIdAsync(key);
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

            announcement.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Announcement>().AddAsync(announcement);
            await _unitOfWork.CompleteAsync();

            // SignalR ile duyuruyu yayınla
            await _announcementService.BroadcastAnnouncementAsync(announcement);

            return Created(announcement);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Announcement update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var announcement = await _unitOfWork.Repository<Announcement>().GetByIdAsync(key);
            if (announcement == null)
            {
                return NotFound();
            }

            update.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Announcement>().Update(update);
            await _unitOfWork.CompleteAsync();

            // SignalR ile güncellenmiş duyuruyu yayınla
            await _announcementService.BroadcastAnnouncementAsync(update);

            return Updated(update);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var announcement = await _unitOfWork.Repository<Announcement>().GetByIdAsync(key);
            if (announcement == null)
            {
                return NotFound();
            }

            _unitOfWork.Repository<Announcement>().Delete(announcement);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
} 