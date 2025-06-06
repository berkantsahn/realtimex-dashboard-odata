using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;
using System.Linq;

namespace RealtimeX.Dashboard.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var repository = _unitOfWork.GetRepository<User>();
            return await repository.GetAllAsync();
        }

        public async Task<(bool success, string token)> LoginAsync(string username, string password)
        {
            var repository = _unitOfWork.GetRepository<User>();
            var user = (await repository.FindAsync(u => u.Username == username)).FirstOrDefault();

            if (user == null)
                return (false, null);

            if (!VerifyPasswordHash(password, user.PasswordHash))
                return (false, null);

            var token = GenerateJwtToken(user);
            return (true, token);
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            var repository = _unitOfWork.GetRepository<User>();
            var existingUser = (await repository.FindAsync(u => u.Username == user.Username || u.Email == user.Email)).FirstOrDefault();

            if (existingUser != null)
                return false;

            user.PasswordHash = HashPassword(password);
            await repository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var repository = _unitOfWork.GetRepository<User>();
            return await repository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var repository = _unitOfWork.GetRepository<User>();
            await repository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var repository = _unitOfWork.GetRepository<User>();
            var user = await repository.GetByIdAsync(userId.ToString());
            if (user == null || !VerifyPasswordHash(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = HashPassword(newPassword);
            await repository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            // Implement password verification logic here
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        private string HashPassword(string password)
        {
            // Implement password hashing logic here
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
} 