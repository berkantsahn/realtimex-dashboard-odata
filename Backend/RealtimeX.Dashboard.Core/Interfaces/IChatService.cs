using System.Collections.Generic;
using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface IChatService
    {
        Task<ChatMessage> SaveMessageAsync(ChatMessage message);
        Task<ChatMessage> SaveMessageAsync(string senderId, string receiverId, string content);
        Task<ChatMessage> GetMessageByIdAsync(string messageId);
        Task<ChatMessage> MarkAsReadAsync(string messageId);
        Task<IEnumerable<ChatMessage>> GetUserMessagesAsync(string userId, string? otherUserId = null);
        Task<IEnumerable<ChatMessage>> GetUnreadMessagesAsync(string userId);
        Task<bool> DeleteMessageAsync(string messageId, string userId);
        Task<IEnumerable<Chat>> GetUserChatsAsync(string userId);
        Task<IEnumerable<ChatMessage>> GetChatMessagesAsync(string chatId);
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string userId);
    }
} 