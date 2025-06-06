using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChatMessage> SaveMessageAsync(ChatMessage message)
        {
            var repository = _unitOfWork.GetRepository<ChatMessage>();
            await repository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            var chat = await GetChatAsync(message.ChatId);
            if (chat != null)
            {
                chat.LastMessage = message.Content;
                chat.LastMessageTime = message.CreatedAt;
                await UpdateChatAsync(chat);
            }

            return message;
        }

        public async Task<ChatMessage> SaveMessageAsync(string senderId, string receiverId, string content)
        {
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            return await SaveMessageAsync(message);
        }

        public async Task<ChatMessage> GetMessageByIdAsync(string messageId)
        {
            var repository = _unitOfWork.GetRepository<ChatMessage>();
            return await repository.GetByIdAsync(messageId);
        }

        public async Task<ChatMessage> MarkAsReadAsync(string messageId)
        {
            var repository = _unitOfWork.GetRepository<ChatMessage>();
            var message = await repository.GetByIdAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                await repository.UpdateAsync(message);
                await _unitOfWork.SaveChangesAsync();
            }
            return message;
        }

        public async Task<IEnumerable<ChatMessage>> GetUserMessagesAsync(string userId, string? otherUserId = null)
        {
            var repository = _unitOfWork.GetRepository<ChatMessage>();
            var messages = await repository.FindAsync(m => 
                m.SenderId == userId || m.ReceiverId == userId);

            if (!string.IsNullOrEmpty(otherUserId))
            {
                messages = messages.Where(m => 
                    m.SenderId == otherUserId || m.ReceiverId == otherUserId);
            }

            return messages.OrderByDescending(m => m.CreatedAt);
        }

        public async Task<IEnumerable<ChatMessage>> GetUnreadMessagesAsync(string userId)
        {
            var repository = _unitOfWork.GetRepository<ChatMessage>();
            return await repository.FindAsync(m => 
                m.ReceiverId == userId && !m.IsRead);
        }

        public async Task<bool> DeleteMessageAsync(string messageId, string userId)
        {
            var repository = _unitOfWork.GetRepository<ChatMessage>();
            var message = await repository.GetByIdAsync(messageId);
            
            if (message == null || message.SenderId != userId)
                return false;

            await repository.DeleteAsync(message);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Chat>> GetUserChatsAsync(string userId)
        {
            var repository = _unitOfWork.GetRepository<Chat>();
            return await repository.FindAsync(c => c.Participants.Contains(userId));
        }

        public async Task<IEnumerable<ChatMessage>> GetChatMessagesAsync(string chatId)
        {
            var repository = _unitOfWork.GetRepository<ChatMessage>();
            var messages = await repository.FindAsync(m => m.ChatId == chatId);
            return messages.OrderBy(m => m.CreatedAt);
        }

        public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string userId)
        {
            var repository = _unitOfWork.GetRepository<ChatMessage>();
            var messages = await repository.FindAsync(m => 
                m.SenderId == userId || m.ReceiverId == userId);
            return messages.OrderByDescending(m => m.CreatedAt);
        }

        private async Task<Chat> GetChatAsync(string chatId)
        {
            var repository = _unitOfWork.GetRepository<Chat>();
            return await repository.GetByIdAsync(chatId);
        }

        private async Task UpdateChatAsync(Chat chat)
        {
            var repository = _unitOfWork.GetRepository<Chat>();
            await repository.UpdateAsync(chat);
            await _unitOfWork.SaveChangesAsync();
        }
    }
} 