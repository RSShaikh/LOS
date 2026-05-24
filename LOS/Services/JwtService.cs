using LOS.Interfaces;
using LOS.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LOS.Services
{
    public class JwtService : IJwtService
    {
        IConfiguration config;

        public JwtService(IConfiguration configuration)
        {
            config = configuration;
        }

        public string GenerateToken(Customer customer)
        {
            var claims = new[]
            {

                new Claim(ClaimTypes.Name, customer.FirstName),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim("CustomerId", customer.CustomerId.ToString()),
                new Claim(ClaimTypes.Role, customer.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
