using ComputerManagement.BO.Models;
using ComputerManagement.Common.Configs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Lib
{
    public class JwtGenerator : IJwtGenerator
    {
        #region Fields
        private readonly JwtConfig _jwtConfig;
        #endregion

        public JwtGenerator(IOptionsMonitor<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.CurrentValue;
        }
        public string GenerateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));

            var tokenDescription = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                new[]
                    {
                    new Claim("Fullname", user.Fullname),
                    new Claim("Email", user.Email),
                    new Claim("Username", user.Username),
                    new Claim("UserId", user.Id.ToString()),

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),

            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            return accessToken;

        }
    }
}
