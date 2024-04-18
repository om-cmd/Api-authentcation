using Microsoft.IdentityModel.Tokens;
using DomainLayer.Datas;
using DomainLayer.DomainModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Layers.Middleware
{
    public class Authentication
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _db;

        public Authentication(IConfiguration configuration, IUnitOfWork db)
        {
            _configuration = configuration;
            _db = db;
        }

        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string RefreshToken(string oldToken)
        {
            var principal = GetPrincipalFromToken(oldToken);

            // Check if the token is expired
            if (principal.Identity is ClaimsIdentity claimsIdentity &&
                claimsIdentity.FindFirst(ClaimTypes.Expiration) != null &&
                DateTime.TryParse(claimsIdentity.FindFirst(ClaimTypes.Expiration).Value, out var expirationDate) &&
                expirationDate < DateTime.UtcNow)
            {
                // If the token is expired, generate a new token
                var username = principal.Identity.Name;
                var user = new User { UserName = username };
                var newJwtToken = GenerateJwtToken(user);

                // Replace the old token with the new one
                ReplaceToken(oldToken, newJwtToken);

                return newJwtToken;
            }
            else
            {
                // If the token is not expired, return the original token
                return oldToken;
            }
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            if (!(validatedToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        public void ReplaceToken(string oldToken, string newToken)
        {
          
             var user = _db.User.FirstOrDefault(u => u.Token == oldToken);
             if (user != null)
             {
                 user.Token = newToken;
                 _db.Save();
             }
           
        }
    }
}
