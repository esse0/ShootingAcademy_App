using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Auth;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtManager _jwtManager;
        private readonly PasswordHasher _passwordHasher;
        private readonly ApplicationDbContext _db;

        public AuthController(JwtManager jwtManager, PasswordHasher passwordHasher, ApplicationDbContext db)
        {
            _jwtManager = jwtManager;
            _passwordHasher = passwordHasher;
            _db = db;
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
                    _jwtManager.JwtCookieOptions
                );

                string token = JwtManager.GenerateJwtToken(_jwtManager.RefreshToken, user);

                HttpContext.Response.Cookies.Append(
                    "RefreshToken",
                    token,
                    _jwtManager.JwtCookieOptions
                );

                user.RToken = token;

                _db.Users.Update(user);

                await _db.SaveChangesAsync();

                return Results.Json(new FullUserModel()
                {
                    FirstName = user.FirstName,
                    SecoundName = user.SecoundName,
                    PatronymicName = user.PatronymicName,
                    Age = user.Age,
                    Country = user.Country,
                    City = user.City,
                    Address = user.Address,
                    Grade = user.Grade,
                    Email = user.Email,
                    Id = user.Id,
                    Role = user.Role
                });
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

        [HttpPost("signout")]
        public IResult logoutJwt()
        {
            HttpContext.Response.Cookies.Delete("AccessToken");
            HttpContext.Response.Cookies.Delete("RefreshToken");

            return Results.Ok();
        }

        [HttpPost("register")]
        public async Task<IResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (_db.Users.Where(usr => usr.Email == model.email).Any())
                    throw new BaseException("Данная почта занята!");

                // Странно что половина полей пустые
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

                return Results.Json(new FullUserModel()
                {
                    FirstName = user.Entity.FirstName,
                    SecoundName = user.Entity.SecoundName,
                    PatronymicName = user.Entity.PatronymicName,
                    Age = user.Entity.Age,
                    Country = user.Entity.Country,
                    City = user.Entity.City,
                    Address = user.Entity.Address,
                    Grade = user.Entity.Grade,
                    Email = user.Entity.Email,
                    Id = user.Entity.Id,
                    Role = user.Entity.Role
                });
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
    }
}
