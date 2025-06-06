using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;

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

        public async Task SendMessage(ChatMessage message)
        {
            message.SentAt = DateTime.UtcNow;
            message.IsRead = false;
            
            await _chatService.SaveMessageAsync(message);
            
            await Clients.User(message.ReceiverId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task SendMediaMessage(IFormFile file, int receiverId)
        {
            var userId = int.Parse(Context.UserIdentifier);
            var mediaResult = await _mediaService.UploadMediaAsync(file, userId.ToString());

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
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _chatService.SaveMessageAsync(message);
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task MarkAsRead(int messageId)
        {
            var message = await _chatService.MarkAsReadAsync(messageId);
            if (message != null)
            {
                await Clients.User(message.SenderId.ToString())
                    .SendAsync("MessageRead", new { MessageId = messageId, ReadAt = message.ReadAt });
            }
        }

        public async Task DeleteMessage(int messageId)
        {
            var message = await _chatService.GetMessageByIdAsync(messageId);
            if (message != null && !string.IsNullOrEmpty(message.MediaUrl))
            {
                await _mediaService.DeleteMediaAsync(message.MediaUrl);
            }

            await _chatService.DeleteMessageAsync(messageId, int.Parse(Context.UserIdentifier));
            await Clients.Users(new[] { message.SenderId.ToString(), message.ReceiverId.ToString() })
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
            var userId = Context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        private MediaType GetMediaType(string mimeType)
        {
            if (mimeType.StartsWith("image/"))
                return MediaType.Image;
            if (mimeType.StartsWith("video/"))
                return MediaType.Video;
            if (mimeType.StartsWith("audio/"))
                return MediaType.Audio;
            return MediaType.Document;
        }
    }
} 