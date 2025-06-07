using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Auth;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services;
using ShootingAcademy.Services.Media;
using System.Security.Claims;
using System.Text;

namespace ShootingAcademy.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtManager _jwtManager;
        private readonly PasswordHasher _passwordHasher;
        private readonly ApplicationDbContext _db;
        private readonly IImageService _imageService;

        public AuthController(JwtManager jwtManager, PasswordHasher passwordHasher, ApplicationDbContext db, IImageService imageService)
        {
            _jwtManager = jwtManager;
            _passwordHasher = passwordHasher;
            _db = db;
            _imageService = imageService;
        }

        [HttpPost("signin")]
        public async Task<IResult> signinJwt([FromBody] SigninModel model)
        {
            try
            {
                
                User user = await _db.Users.FirstOrDefaultAsync(i => i.Email == model.email) 
                    ?? throw new BaseException("Почта не верна");

                if (!_passwordHasher.Verify(model.password, user.PasswordHash))
                {
                    throw new BaseException("Пароль не верный");
                }

                HttpContext.Response.Cookies.Append(
                    "AccessToken",
                    JwtManager.GenerateJwtToken(_jwtManager.AccessToken, user),
                    _jwtManager.AccessTokenCookieOptions
                );                                                                  

                string token = JwtManager.GenerateJwtToken(_jwtManager.RefreshToken, user);

                HttpContext.Response.Cookies.Append(
                    "RefreshToken",
                    token,
                    _jwtManager.RefreshTokenCookieOptions
                );

                user.RToken = token;
                user.RTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtManager.RefreshToken.ExpiryMinutes);

                _db.Users.Update(user);

                await _db.SaveChangesAsync();

                var profileImage = await _imageService.GetFileUrl(user.Id);

                return Results.Json(UserWithAvatar.FromEntity(user, profileImage.FileUri));
                
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 400);
            }
        }

        [HttpPost("register")]
        public async Task<IResult> Register([FromBody] RegisterModel model)
        {
            try
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

                return Results.Json(UserWithAvatar.FromEntity(user.Entity, ""));
            }
            catch (BaseException exp)
            {
                return Results.Json(exp.GetModel(), statusCode: exp.Code);
            }
            catch
            {
                return Results.Problem("AuthController->Register", statusCode: 400);
            }
        }

        [HttpPost("refresh")]
        public async Task<IResult> RefreshAccessToken()
        {
            try
            {
                var refreshToken = HttpContext.Request.Cookies["RefreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                    return Results.Unauthorized();

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
                    return Results.Unauthorized();

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

                if (user == null || user.RToken != refreshToken)
                    return Results.Unauthorized();

                 if (user?.RTokenExpiry < DateTime.UtcNow)
                    return Results.Unauthorized();

                var newAccessToken = JwtManager.GenerateJwtToken(_jwtManager.AccessToken, user);
                var newRefreshToken = JwtManager.GenerateJwtToken(_jwtManager.RefreshToken, user);

                user.RToken = newRefreshToken;
                user.RTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtManager.RefreshToken.ExpiryMinutes);

                await _db.SaveChangesAsync();

                HttpContext.Response.Cookies.Append("AccessToken", newAccessToken, _jwtManager.AccessTokenCookieOptions);
                HttpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, _jwtManager.RefreshTokenCookieOptions);

                return Results.Ok(new { message = "Токен обновлён" });
            }
            catch (BaseException exp)
            {
                return Results.Json(exp.GetModel(), statusCode: exp.Code);
            }
            catch
            {
                return Results.Problem("AuthController->Refresh", statusCode: 400);
            }
        }

        [HttpPost("signout"), Authorize]
        public async Task<IResult> LogoutJwt()
        {
            try
            {
                var userId = AutorizeData.FromContext(HttpContext).UserGuid;

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return Results.Unauthorized();

                user.RToken = "";
                user.RTokenExpiry = null;

                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                HttpContext.Response.Cookies.Delete("AccessToken");
                HttpContext.Response.Cookies.Delete("RefreshToken");

                return Results.Ok(new { message = "Вы вышли из системы" });
            }
            catch (BaseException exp)
            {
                return Results.Json(exp.GetModel(), statusCode: exp.Code);
            }
            catch
            {
                return Results.Problem("Logout Error", statusCode: 500);
            }
        }
    }
}
