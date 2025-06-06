using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] int? otherUserId = null)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value);
            var messages = await _chatService.GetUserMessagesAsync(userId, otherUserId);
            return Ok(messages);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadMessages()
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value);
            var messages = await _chatService.GetUnreadMessagesAsync(userId);
            return Ok(messages);
        }

        [HttpPost("mark-read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var message = await _chatService.MarkAsReadAsync(messageId);
            if (message == null)
            {
                return NotFound();
            }
            return Ok(message);
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value);
            var success = await _chatService.DeleteMessageAsync(messageId, userId);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }
    }
} 