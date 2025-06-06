using System.Collections.Generic;
using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface IChatService
    {
        Task<ChatMessage> SaveMessageAsync(ChatMessage message);
        Task<ChatMessage> GetMessageByIdAsync(int messageId);
        Task<ChatMessage> MarkAsReadAsync(int messageId);
        Task<IEnumerable<ChatMessage>> GetUserMessagesAsync(int userId, int? otherUserId = null);
        Task<IEnumerable<ChatMessage>> GetUnreadMessagesAsync(int userId);
        Task<bool> DeleteMessageAsync(int messageId, int userId);
    }
} 