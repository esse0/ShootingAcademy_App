using Microsoft.IdentityModel.Tokens;
using ShootingAcademy.Models;
using ShootingAcademy.Models.DB.ModelUser;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShootingAcademy.Services
{
    public class JwtManager
    {
        public JwtSettings AccessToken { get; private set; }
        public JwtSettings RefreshToken { get; private set; }

        public CookieOptions AccessTokenCookieOptions { get; private set; }
        public CookieOptions RefreshTokenCookieOptions { get; private set; }

        public JwtManager(JwtSettings accessToken, JwtSettings refreshToken)
        {
            RefreshToken = refreshToken;
            AccessToken = accessToken;

            AccessTokenCookieOptions = new()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(AccessToken.ExpiryMinutes)
            };

            RefreshTokenCookieOptions = new()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(RefreshToken.ExpiryMinutes)
            };
        }

        public static string GenerateJwtToken(JwtSettings settings, User user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SecretKey));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static TokenValidationParameters GetParameters(JwtSettings settings)
        {
            byte[] key = Encoding.ASCII.GetBytes(settings.SecretKey);

            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings.Issuer,
                ValidAudience = settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            };
        }

        public static ClaimsPrincipal? ValidateToken(string token, TokenValidationParameters parameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                return tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
