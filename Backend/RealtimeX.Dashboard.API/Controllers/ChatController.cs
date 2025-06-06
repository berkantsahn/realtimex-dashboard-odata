using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;
using System.Security.Claims;

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
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetUserMessages([FromQuery] string? otherUserId = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var messages = await _chatService.GetUserMessagesAsync(userId, otherUserId);
            return Ok(messages);
        }

        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetUnreadMessages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var messages = await _chatService.GetUnreadMessagesAsync(userId);
            return Ok(messages);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var messages = await _chatService.GetChatHistoryAsync(userId);
            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<ActionResult<ChatMessage>> SendMessage([FromBody] ChatMessage message)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            message.SenderId = userId;
            message.CreatedAt = DateTime.UtcNow;
            message.SentAt = DateTime.UtcNow;

            var savedMessage = await _chatService.SaveMessageAsync(message);
            return Ok(savedMessage);
        }

        [HttpGet("chats")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetUserChats()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var chats = await _chatService.GetUserChatsAsync(userId);
            return Ok(chats);
        }

        [HttpGet("chat/{chatId}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatMessages(string chatId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var messages = await _chatService.GetChatMessagesAsync(chatId);
            return Ok(messages);
        }

        [HttpPost("message/{messageId}/read")]
        public async Task<ActionResult<ChatMessage>> MarkAsRead(string messageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var message = await _chatService.MarkAsReadAsync(messageId);
            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpDelete("message/{messageId}")]
        public async Task<ActionResult> DeleteMessage(string messageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _chatService.DeleteMessageAsync(messageId, userId);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
} 