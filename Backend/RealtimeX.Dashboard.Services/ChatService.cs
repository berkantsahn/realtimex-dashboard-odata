using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            await _unitOfWork.Repository<ChatMessage>().AddAsync(message);
            await _unitOfWork.CompleteAsync();
            return message;
        }

        public async Task<ChatMessage> GetMessageByIdAsync(int messageId)
        {
            var messages = await _unitOfWork.Repository<ChatMessage>()
                .FindAsync(m => m.Id == messageId);
            return messages.FirstOrDefault();
        }

        public async Task<ChatMessage> MarkAsReadAsync(int messageId)
        {
            var message = await GetMessageByIdAsync(messageId);
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                await _unitOfWork.Repository<ChatMessage>().UpdateAsync(message);
                await _unitOfWork.CompleteAsync();
            }
            return message;
        }

        public async Task<IEnumerable<ChatMessage>> GetUserMessagesAsync(int userId, int? otherUserId = null)
        {
            var query = _unitOfWork.Repository<ChatMessage>().Query()
                .Where(m => m.SenderId == userId || m.ReceiverId == userId);

            if (otherUserId.HasValue)
            {
                query = query.Where(m => 
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId));
            }

            return await query
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetUnreadMessagesAsync(int userId)
        {
            return await _unitOfWork.Repository<ChatMessage>().Query()
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteMessageAsync(int messageId, int userId)
        {
            var message = await GetMessageByIdAsync(messageId);
            if (message != null && message.SenderId == userId)
            {
                await _unitOfWork.Repository<ChatMessage>().DeleteAsync(message);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            return false;
        }
    }
} 