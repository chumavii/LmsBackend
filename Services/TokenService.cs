using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LmsApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace LmsApi.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        public string CreateToken(ApplicationUser user, IList<string> roles)
        {

            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            if (user.UserName == null)
                throw new ArgumentNullException(nameof(user), "Username cannot be null");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FullName", user.FullName ?? "")
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var keyValue = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyValue))
                throw new Exception("JWT key is missing from config");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }


}
