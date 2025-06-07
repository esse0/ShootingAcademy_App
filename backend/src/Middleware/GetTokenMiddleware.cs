using ShootingAcademy.Models;
using ShootingAcademy.Services;
using System.Security.Claims;
using ShootingAcademy.Models.DB.ModelUser;
using Microsoft.EntityFrameworkCore;

namespace ShootingAcademy.Middleware
{
    public class GetTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtManager _jwtManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GetTokenMiddleware(RequestDelegate next, JwtManager jwtManager, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _jwtManager = jwtManager;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string? access = httpContext.Request.Cookies["AccessToken"];
            string? refresh = httpContext.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refresh))
            {
                await _next(httpContext);
                return;
            }

            var refreshJwtClaims = JwtManager.ValidateToken(refresh, JwtManager.GetParameters(_jwtManager.RefreshToken));
            if (refreshJwtClaims == null)
            {
                await _next(httpContext);
                return;
            }

            var claims = refreshJwtClaims.Claims.ToList();
            Guid userId = Guid.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            string role = claims.First(c => c.Type == ClaimTypes.Role).Value;

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.RToken != refresh)
            {
                await _next(httpContext);
                return;
            }

            var accessJwtClaims = JwtManager.ValidateToken(access, JwtManager.GetParameters(_jwtManager.AccessToken));
            if (string.IsNullOrEmpty(access) || accessJwtClaims == null)
            {
                var newAccessToken = JwtManager.GenerateJwtToken(_jwtManager.AccessToken, user);
                var newRefreshToken = JwtManager.GenerateJwtToken(_jwtManager.RefreshToken, user);

                user.RToken = newRefreshToken;
                user.RTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtManager.RefreshToken.ExpiryMinutes);
                await dbContext.SaveChangesAsync();

                httpContext.Response.Cookies.Append("AccessToken", newAccessToken, _jwtManager.AccessTokenCookieOptions);
                httpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, _jwtManager.RefreshTokenCookieOptions);

                access = newAccessToken;
            }

            httpContext.Request.Headers["Authorization"] = $"Bearer {access}";

            await _next(httpContext);
        }
    }

    public static class GetTokenExtensions
    {
        public static IApplicationBuilder UseGetToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GetTokenMiddleware>();
        }
    }
}
