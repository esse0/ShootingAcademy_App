using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Auth;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services.Media;
using System.Security.Claims;
using System.Text;

namespace ShootingAcademy.Services
{
    public interface IAuthService
    {
        Task<UserWithAvatar> SignInAsync(SigninModel model, HttpContext httpContext);
        Task<UserWithAvatar> RegisterAsync(RegisterModel model);
        Task<object> RefreshAccessTokenAsync(HttpContext httpContext);
        Task<object> LogoutAsync(HttpContext httpContext);
    }

    public class AuthService : IAuthService
    {
        private readonly JwtManager _jwtManager;
        private readonly PasswordHasher _passwordHasher;
        private readonly ApplicationDbContext _db;
        private readonly IImageService _imageService;

        public AuthService(JwtManager jwtManager, PasswordHasher passwordHasher, ApplicationDbContext db, IImageService imageService)
        {
            _jwtManager = jwtManager;
            _passwordHasher = passwordHasher;
            _db = db;
            _imageService = imageService;
        }

        public async Task<UserWithAvatar> SignInAsync(SigninModel model, HttpContext httpContext)
        {
            User user = await _db.Users.FirstOrDefaultAsync(i => i.Email == model.email) 
                ?? throw new BaseException("Данные не верны");

            if (!_passwordHasher.Verify(model.password, user.PasswordHash))
            {
                throw new BaseException("Данные не верны");
            }

            string accessToken = JwtManager.GenerateJwtToken(_jwtManager.AccessToken, user);
            string refreshToken = JwtManager.GenerateJwtToken(_jwtManager.RefreshToken, user);

            httpContext.Response.Cookies.Append(
                "AccessToken",
                accessToken,
                _jwtManager.AccessTokenCookieOptions
            );

            httpContext.Response.Cookies.Append(
                "RefreshToken",
                refreshToken,
                _jwtManager.RefreshTokenCookieOptions
            );

            user.RToken = refreshToken;
            user.RTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtManager.RefreshToken.ExpiryMinutes);

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            var profileImage = await _imageService.GetFileUrl(user.Id);
            return UserWithAvatar.FromEntity(user, profileImage.FileUri);
        }

        public async Task<UserWithAvatar> RegisterAsync(RegisterModel model)
        {
            if (_db.Users.Where(usr => usr.Email == model.email).Any())
                throw new BaseException("Данная почта занята!");

            var user = await _db.Users.AddAsync(new User()
            {
                FirstName = model.name,
                SecoundName = model.lastName,
                PatronymicName = string.Empty,
                Email = model.email,
                PasswordHash = _passwordHasher.Hash(model.password),
                Role = "athlete",
                Grade = "",
                Age = 0,
                Country = "",
                City = "",
                Address = "",
                RToken = ""
            });

            await _db.SaveChangesAsync();
            return UserWithAvatar.FromEntity(user.Entity, "");
        }

        public async Task<object> RefreshAccessTokenAsync(HttpContext httpContext)
        {
            var refreshToken = httpContext.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                throw new BaseException("Токен обновления отсутствует", 401);

            var principal = JwtManager.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = _jwtManager.RefreshToken.Issuer,
                ValidAudience = _jwtManager.RefreshToken.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtManager.RefreshToken.SecretKey)),
                ClockSkew = TimeSpan.Zero
            });

            if (principal == null)
                throw new BaseException("Недействительный токен", 401);

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new BaseException("Недействительный токен", 401);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            if (user == null || user.RToken != refreshToken || user.RTokenExpiry < DateTime.UtcNow)
                throw new BaseException("Недействительный токен", 401);

            var newAccessToken = JwtManager.GenerateJwtToken(_jwtManager.AccessToken, user);
            var newRefreshToken = JwtManager.GenerateJwtToken(_jwtManager.RefreshToken, user);

            user.RToken = newRefreshToken;
            user.RTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtManager.RefreshToken.ExpiryMinutes);

            await _db.SaveChangesAsync();

            httpContext.Response.Cookies.Append("AccessToken", newAccessToken, _jwtManager.AccessTokenCookieOptions);
            httpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, _jwtManager.RefreshTokenCookieOptions);

            return new { message = "Токен обновлён" };
        }

        public async Task<object> LogoutAsync(HttpContext httpContext)
        {
            var userId = AutorizeData.FromContext(httpContext).UserGuid;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new BaseException("Пользователь не найден", 401);

            user.RToken = "";
            user.RTokenExpiry = null;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            httpContext.Response.Cookies.Delete("AccessToken");
            httpContext.Response.Cookies.Delete("RefreshToken");

            return new { message = "Вы вышли из системы" };
        }
    }
} 