using ShootingAcademy.Models;
using ShootingAcademy.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ShootingAcademy.Models.DB.ModelUser;

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

        public Task Invoke(HttpContext httpContext)
        {
            string? access = httpContext.Request.Cookies["AccessToken"];
            string? refresh = httpContext.Request.Cookies["RefreshToken"];

            if (refresh == null || string.IsNullOrEmpty(refresh))
                return _next(httpContext);

            ClaimsPrincipal? refreshJwtClaims = JwtManager.ValidateToken(refresh, JwtManager.GetParameters(_jwtManager.RefreshToken));

            if (refreshJwtClaims == null)
                return _next(httpContext);

            var claims = refreshJwtClaims.Claims.Select(i => new { i.Type, i.Value }).ToList();

            Guid userId = Guid.Parse(claims[0].Value);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContect = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var user = dbContect.Users.FirstOrDefault(u => u.Id == userId);
               
                if (user == null)
                    return _next(httpContext);

                if (user.RToken != refresh)
                    return _next(httpContext);
            }

            ClaimsPrincipal? accessJwtClaims = JwtManager.ValidateToken(access, JwtManager.GetParameters(_jwtManager.AccessToken));

            if (access == null || string.IsNullOrEmpty(access) || accessJwtClaims == null)
            {
                access = JwtManager.GenerateJwtToken(_jwtManager.AccessToken, new User()
                {
                    Id = Guid.Parse(claims[0].Value),
                    Role = claims[1].Value
                });

                httpContext.Response.Cookies.Append("AccessToken", access);
            }

            httpContext.Request.Headers.Append("Authorization", $"Bearer {access}");

            return _next(httpContext);
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
