using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.User;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet("auth"), Authorize]
        public IResult Auth()
        {
            return Results.Ok();
        }

        [HttpPut, Authorize]
        public async Task<IResult> Put([FromBody] UserProfileData profileData)
        {
            try
            {
                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                if (dbContext.Users.Where(usr => usr.Id != userGuid && usr.Email == profileData.Email).Any())
                    throw new BaseException("Данная почта занята!");

                User user = await dbContext.Users.FirstAsync(usr => usr.Id == userGuid);

                user.FirstName = profileData.Name;
                user.SecoundName = profileData.LastName;
                user.Address = profileData.Address;
                user.City = profileData.City;
                user.Country = profileData.Country;
                user.Grade = profileData.Grade;
                user.Age = profileData.Age;
                user.Email = profileData.Email;

                dbContext.Users.Update(user);

                await dbContext.SaveChangesAsync();

                return Results.Json(FullUserModel.FromEntity(user));
            }
            catch (BaseException exp)
            {
                return Results.Json(exp.GetModel(), statusCode: exp.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 400);
            }
        }

        [HttpGet("get"), Authorize(Roles = "admin")]
        public async Task<IResult> GetUsersWithoutAdmin()
        {
            try
            {
                var users = await dbContext.Users
                    .Where(u => u.Role != "admin")
                    .AsNoTracking()
                    .ToListAsync();

                var result = users.Select(FullUserModel.FromEntity).ToList();

                return Results.Json(result);
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

        [HttpPost("changerole"), Authorize(Roles = "admin")]
        public async Task<IResult> ChangeUserRole([FromQuery] string userId, [FromQuery] string newRole)
        {
            try
            {
                string[] allowedRoles = ["athlete", "moderator", "coach", "organisator"];

                if (!allowedRoles.Any(role => role == newRole))
                    throw new BaseException("Role not supported", 400);

                var user = await dbContext.Users.FindAsync(Guid.Parse(userId)) 
                                 ?? throw new BaseException("User not found", 404);

                if (user.Role == "admin" && newRole != "admin")
                    throw new BaseException("Cannot change administrator role", 400);

                user.Role = newRole;

                await dbContext.SaveChangesAsync();

                return Results.Ok();
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

    }
}
