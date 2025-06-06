using System.Threading.Tasks;
using RealtimeX.Dashboard.Core.Entities;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface IAuthService
    {
        Task<(bool success, string token)> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(User user, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ValidateTokenAsync(string token);
    }
} 