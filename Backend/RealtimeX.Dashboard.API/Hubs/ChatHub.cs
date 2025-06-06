using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;
using System.Security.Claims;

namespace RealtimeX.Dashboard.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IMediaService _mediaService;

        public ChatHub(IChatService chatService, IMediaService mediaService)
        {
            _chatService = chatService;
            _mediaService = mediaService;
        }

        public async Task SendMessage(string message, string recipientId)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
            {
                throw new HubException("Invalid sender ID");
            }

            if (string.IsNullOrEmpty(recipientId))
            {
                throw new HubException("Invalid recipient ID");
            }

            var chatMessage = await _chatService.SaveMessageAsync(senderId, recipientId, message);
            if (chatMessage != null)
            {
                await Clients.Group($"user_{recipientId}").SendAsync("ReceiveMessage", chatMessage);
                await Clients.Caller.SendAsync("MessageSent", chatMessage);
            }
        }

        public async Task SendMediaMessage(IFormFile file, string receiverId)
        {
            var userId = Context.UserIdentifier;
            var mediaResult = await _mediaService.UploadMediaAsync(file, userId);

            var message = new ChatMessage
            {
                SenderId = userId,
                ReceiverId = receiverId,
                Content = mediaResult.Name,
                MediaUrl = mediaResult.Url,
                MediaThumbnailUrl = mediaResult.ThumbnailUrl,
                MediaSize = mediaResult.Size,
                MediaName = mediaResult.Name,
                MediaMimeType = mediaResult.MimeType,
                MediaDuration = mediaResult.Duration,
                Type = GetMediaType(mediaResult.MimeType),
                CreatedAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _chatService.SaveMessageAsync(message);
            await Clients.User(receiverId).SendAsync("ReceiveMessage", message);
        }

        public async Task MarkAsRead(string messageId)
        {
            var message = await _chatService.MarkAsReadAsync(messageId);
            if (message != null)
            {
                await Clients.User(message.SenderId)
                    .SendAsync("MessageRead", new { MessageId = messageId, ReadAt = message.ReadAt });
            }
        }

        public async Task DeleteMessage(string messageId)
        {
            var message = await _chatService.GetMessageByIdAsync(messageId);
            if (message != null && !string.IsNullOrEmpty(message.MediaUrl))
            {
                await _mediaService.DeleteMediaAsync(message.MediaUrl);
            }

            await _chatService.DeleteMessageAsync(messageId, Context.UserIdentifier);
            await Clients.Users(new[] { message.SenderId, message.ReceiverId })
                .SendAsync("MessageDeleted", messageId);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("Invalid user ID");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        private string GetMediaType(string mimeType)
        {
            if (mimeType.StartsWith("image/"))
                return "image";
            if (mimeType.StartsWith("video/"))
                return "video";
            if (mimeType.StartsWith("audio/"))
                return "audio";
            return "document";
        }
    }
} 