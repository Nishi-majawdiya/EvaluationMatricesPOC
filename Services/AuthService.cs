using EvaluationMatricesPOC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationMatricesPOC.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        public async Task<object> RegisterAsync(RegisterModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return new { message = "User already exists ❌" };

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new { message = "User registered successfully ✅" };
        }

        /// <summary>
        /// Authenticates a user and returns a token asynchronously.
        /// </summary>
        public async Task<object> LoginAsync(LoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == model.Email && u.Password == model.Password);

            if (user == null)
                return new { message = "Invalid credentials ❌" };

            var token = GenerateJwtToken(user);

            return new
            {
                token = token,
                message = "Login successful ✅"
            };
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}