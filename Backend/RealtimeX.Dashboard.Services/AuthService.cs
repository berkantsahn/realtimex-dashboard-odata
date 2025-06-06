using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;

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

        public async Task<(bool success, string token)> LoginAsync(string username, string password)
        {
            var user = await _unitOfWork.Repository<User>().FindAsync(u => u.Username == username);
            var userEntity = user.FirstOrDefault();

            if (userEntity == null || !VerifyPasswordHash(password, userEntity.PasswordHash))
            {
                return (false, null);
            }

            var token = GenerateJwtToken(userEntity);
            return (true, token);
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            if (await _unitOfWork.Repository<User>().FindAsync(u => u.Username == user.Username) != null)
            {
                return false;
            }

            user.PasswordHash = HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            if (user == null || !VerifyPasswordHash(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = HashPassword(newPassword);
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

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

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
} 