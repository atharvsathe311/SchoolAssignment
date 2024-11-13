using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserAPI.Business.Data;
using UserAPI.Business.Models;
using UserAPI.Business.Services.Interfaces;
using UserAPI.Core.GeneralModels;

namespace UserAPI.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserAPIDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(UserAPIDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string Login(LoginRequest loginRequest,User user)
        {
            var claims = new List<Claim>
            {
                new("Username", user.Username),
                new("UserId", user.UserId.ToString())
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            if (!user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Teacher"));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}