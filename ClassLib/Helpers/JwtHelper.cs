using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ClassLib.DTO.User;
using ClassLib.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ClassLib.Repositories;

namespace ClassLib.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly UserRepository _userRepository;

        public JwtHelper(IConfiguration configuration, UserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public (string, string, string) generateToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            var key = Encoding.UTF8.GetBytes(secretKey);
            //var key = Convert.FromBase64String(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    ////new Claim(JwtRegisteredClaimNames.Email, user.Id.ToString()),
                    //new Claim(ClaimTypes.Name, user.Username),
                    //new Claim(ClaimTypes.Role, user.Role),

                    new Claim(JwtRegisteredClaimNames.Jti, Math.Abs(BitConverter.ToInt64(Guid.NewGuid().ToByteArray())).ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Username", user.Username),
                    new Claim("Name", user.Name),
                    new Claim("DateOfBirth", user.DateOfBirth.ToString()),
                    new Claim("Gender", (user.Gender == 0 ? "Male" : "Female").ToString()),
                    new Claim("Gmail", user.Gmail),
                    new Claim("PhoneNumber", user.PhoneNumber),
                    new Claim("Role", user.Role),
                    new Claim("Avatar", user.Avatar),
                    new Claim("CreateAt", user.CreatedAt.ToString()),
                    new Claim("Status", user.Status),

                }),

                // time to expire is 3 hours
                //Expires = DateTime.UtcNow.AddSeconds(20),

                Expires = Helpers.TimeProvider.GetVietnamNow().AddHours(3),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenId = token.Id;

            var accessToken = tokenHandler.WriteToken(token);
            var refreshToken = generateRefreshToken();


            return (tokenId, accessToken, refreshToken);
        }

        public string generateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        public DateTime ConvertUnixTimeToDateTime(long utcExpiredDate)
        {
            var datTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
             return datTimeInterval.AddSeconds(utcExpiredDate).ToUniversalTime();
        }

        public string? getSecretKey()
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            return secretKey;
        }

    }
}
