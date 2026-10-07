using backend.Data;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public class AuthService(AppDbContext db, ILogger<AuthService> logger, IConfiguration config) : IAuthService
    {
        public async Task Register(RegisterRequest request)
        {
            var exist = await db.Users.AnyAsync(u => u.Name.ToLower() == request.Name.ToLower());

            if (exist)
            {
                logger.LogWarning("User with name {Name} already exists", request.Name);
                throw new InvalidOperationException("User with this name already exists");
            }

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new UserEntity
            {
                Name = request.Name,
                HashPassword = hashPassword
            };

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            logger.LogInformation("User {Name} added", newUser.Name);
        }

        public async Task<string> Login(LoginRequest request)
        {
            var entity = await db.Users.FirstOrDefaultAsync(u => u.Name.ToLower() == request.Name.ToLower());

            if (entity == null)
            {
                logger.LogWarning("User {name} does not exist", request.Name);
                throw new UnauthorizedAccessException("User doest not exist");
            }

            if(!BCrypt.Net.BCrypt.Verify(request.Password, entity.HashPassword))
            {
                logger.LogWarning("Wrong password for {name}", request.Name);
                throw new UnauthorizedAccessException("Wrong password");
            }

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, entity.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, entity.Name),
            new Claim(ClaimTypes.Role, entity.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT_KEY"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["JWT_ISSUER"],
                audience: config["JWT_AUDIENCE"],
                claims: claims,
                expires: DateTime.Now.AddDays(double.Parse(config["JWT_EXPIRE_DAYS"]!)),
                signingCredentials: creds);

            logger.LogInformation("User {Name} has logged", request.Name);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
