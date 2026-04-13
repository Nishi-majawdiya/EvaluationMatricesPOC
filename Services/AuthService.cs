using EvaluationMatricesPOC.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EvaluationMatricesPOC.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ✅ REGISTER
        public object Register(RegisterModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
                return new { message = "User already exists ❌" };

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return new { message = "User registered successfully ✅" };
        }

        // ✅ LOGIN
        public object Login(LoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u =>
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

        // ✅ JWT TOKEN
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