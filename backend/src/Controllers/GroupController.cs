using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.AthleteGroup;
using ShootingAcademy.Models.Controllers.Group;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services;
using System.Data;


namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GroupController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IResult> Get()
        {
            try
            {
                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var athleteGroups = await _context.AthleteGroups
                    .Include(g => g.Coach)
                    .AsNoTracking()
                    .ToListAsync();

                var result = athleteGroups.Select(group => new AthleteGroupModel
                {
                    id = group.Id.ToString(),
                    organisationName = group.OrganisationName,
                    coach = new FullUserModel
                    {
                        Id = group.Coach.Id,
                        FirstName = group.Coach.FirstName,
                        SecoundName = group.Coach.SecoundName,
                        Email = group.Coach.Email,
                        PatronymicName = group.Coach.PatronymicName,
                        Age = group.Coach.Age,
                        Grade = group.Coach.Grade,
                        Country = group.Coach.Country,
                        City = group.Coach.City,
                        Address = group.Coach.Address,
                        Role = group.Coach.Role
                    }
                }).ToList();

                return Results.Json(result);
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }

        [HttpGet("user")]
        public async Task<IResult> GetUserGroups()
        {
            try
            {
                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var athleteGroups = await _context.AthleteGroups
                .Include(g => g.Coach)
                .Include(g => g.Athletes)
                .Where(g => g.Athletes.Any(a => a.AthleteId == userGuid))
                .AsNoTracking()
                .ToListAsync();

                var result = athleteGroups.Select(group => new AthleteGroupModel
                {
                    id = group.Id.ToString(),
                    organisationName = group.OrganisationName,
                    coach = new FullUserModel
                    {
                        Id = group.Coach.Id,
                        FirstName = group.Coach.FirstName,
                        SecoundName = group.Coach.SecoundName,
                        Email = group.Coach.Email,
                        PatronymicName = group.Coach.PatronymicName,
                        Age = group.Coach.Age,
                        Grade = group.Coach.Grade,
                        Country = group.Coach.Country,
                        City = group.Coach.City,
                        Address = group.Coach.Address,
                        Role = group.Coach.Role
                    }
                }).ToList();

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


        [HttpGet("coach"), Authorize(Roles = "coach")]
        public async Task<IResult> GetCoachGroups()
        {
            try
            {
                Guid coachId = AutorizeData.FromContext(HttpContext).UserGuid;

                var athleteGroups = await _context.AthleteGroups
                    .Where(g => g.CoachId == coachId)
                    .Include(g => g.Coach)
                    .AsNoTracking()
                    .ToListAsync();

                var result = athleteGroups.Select(group => new AthleteGroupModel
                {
                    id = group.Id.ToString(),
                    organisationName = group.OrganisationName,
                    coach = new FullUserModel
                    {
                        Id = group.Coach.Id,
                        FirstName = group.Coach.FirstName,
                        SecoundName = group.Coach.SecoundName,
                        Email = group.Coach.Email,
                        PatronymicName = group.Coach.PatronymicName,
                        Age = group.Coach.Age,
                        Grade = group.Coach.Grade,
                        Country = group.Coach.Country,
                        City = group.Coach.City,
                        Address = group.Coach.Address,
                        Role = group.Coach.Role
                    }
                }).ToList();

                return Results.Json(result);
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }


        [HttpGet("user/fulldata")]
        public async Task<IResult> GetUserGroup([FromQuery] string groupId)
        {
            try
            {
                if (!Guid.TryParse(groupId, out Guid groupGuid))
                {
                    throw new BaseException("Invalid group ID format.", code: 400);
                }

                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var group = await _context.AthleteGroups
                    .Where(g => g.Id == groupGuid)
                    .Include(g => g.Coach)
                    .Include(g => g.Athletes)
                    .ThenInclude(a => a.Athlete)
                    .Where(g => g.Athletes.Any(a => a.AthleteId == userGuid))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (group == null)
                {
                    throw new BaseException("Group not found.", code: 404);
                }

                var result = new AthleteGroupModel
                {
                    id = group.Id.ToString(),
                    organisationName = group.OrganisationName,
                    coach = new FullUserModel
                    {
                        Id = group.Coach.Id,
                        FirstName = group.Coach.FirstName,
                        SecoundName = group.Coach.SecoundName,
                        Email = group.Coach.Email,
                        PatronymicName = group.Coach.PatronymicName ?? "",
                        Age = group.Coach.Age,
                        Grade = group.Coach.Grade ?? "",
                        Country = group.Coach.Country ?? "",
                        City = group.Coach.City ?? "",
                        Address = group.Coach.Address ?? "",
                        Role = group.Coach.Role
                    },
                    members = group.Athletes?
                    .Where(a => a.Athlete != null)
                    .Select(a => new FullUserModel
                    {
                        Id = a.Athlete.Id,
                        FirstName = a.Athlete.FirstName,
                        SecoundName = a.Athlete.SecoundName,
                        Email = a.Athlete.Email,
                        PatronymicName = a.Athlete.PatronymicName ?? "",
                        Age = a.Athlete.Age,
                        Grade = a.Athlete.Grade ?? "",
                        Country = a.Athlete.Country ?? "",
                        City = a.Athlete.City ?? "",
                        Address = a.Athlete.Address ?? "",
                        Role = a.Athlete.Role
                    })
                    .ToList() ?? new List<FullUserModel>()
                };

                return Results.Json(result);
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }

        [HttpGet("coach/fulldata"), Authorize(Roles = "coach")]
        public async Task<IResult> GetCoachGroup([FromQuery] string groupId)
        {
            try
            {
                if (!Guid.TryParse(groupId, out Guid groupGuid))
                {
                    throw new BaseException("Invalid group ID format.", code: 400);
                }

                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var group = await _context.AthleteGroups
                    .Where(g => g.Id == groupGuid)
                    .Include(g => g.Coach)
                    .Where(g => g.CoachId == userGuid)
                    .Include(g => g.Athletes)
                    .ThenInclude(a => a.Athlete)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (group == null)
                {
                    throw new BaseException("Group not found.", code: 404);
                }

                var result = new AthleteGroupModel
                {
                    id = group.Id.ToString(),
                    organisationName = group.OrganisationName,
                    coach = new FullUserModel
                    {
                        Id = group.Coach.Id,
                        FirstName = group.Coach.FirstName,
                        SecoundName = group.Coach.SecoundName,
                        Email = group.Coach.Email,
                        PatronymicName = group.Coach.PatronymicName ?? "",
                        Age = group.Coach.Age,
                        Grade = group.Coach.Grade ?? "",
                        Country = group.Coach.Country ?? "",
                        City = group.Coach.City ?? "",
                        Address = group.Coach.Address ?? "",
                        Role = group.Coach.Role
                    },
                    members = group.Athletes?
                    .Where(a => a.Athlete != null)
                    .Select(a => new FullUserModel
                    {
                        Id = a.Athlete.Id,
                        FirstName = a.Athlete.FirstName,
                        SecoundName = a.Athlete.SecoundName,
                        Email = a.Athlete.Email,
                        PatronymicName = a.Athlete.PatronymicName ?? "",
                        Age = a.Athlete.Age,
                        Grade = a.Athlete.Grade ?? "",
                        Country = a.Athlete.Country ?? "",
                        City = a.Athlete.City ?? "",
                        Address = a.Athlete.Address ?? "",
                        Role = a.Athlete.Role
                    })
                    .ToList() ?? new List<FullUserModel>()
                };

                return Results.Json(result);
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }


        [HttpPost("addmember"), Authorize(Roles = "coach")]
        public async Task<IResult> AddMember([FromQuery] MemberData data)
        {
            try
            {
                if (!Guid.TryParse(data.groupId, out Guid groupGuid) || !Guid.TryParse(data.userId, out Guid userGuid))
                {
                    throw new BaseException("Invalid group ID or user ID format.", code: 400);
                }

                Guid coachGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var group = await _context.AthleteGroups
                    .Include(g => g.Athletes)
                    .FirstOrDefaultAsync(g => g.Id == groupGuid && g.CoachId == coachGuid);

                if (group == null)
                {
                    throw new BaseException("Group not found or you don't have permission to modify it.", code: 404);
                }

                var user = await _context.Users.FindAsync(userGuid);

                if (user == null)
                {
                    throw new BaseException("User not found.", code: 404);
                }

                if (user.Role != "athlete")
                {
                    throw new BaseException("Don't have premission to add coaches", code: 400);
                }

                if (group.Athletes.Any(a => a.AthleteId == userGuid))
                {
                    throw new BaseException("User is already a member of this group.", code: 400);
                }

                var groupMember = new GroupMember
                {
                    Id = Guid.NewGuid(),
                    AthleteGroupId = groupGuid,
                    AthleteId = userGuid
                };

                group.Athletes.Add(groupMember);
                _context.GroupMembers.Add(groupMember);

                await _context.SaveChangesAsync();

                return Results.Created();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }


        [HttpPost("kickmember"), Authorize(Roles = "coach")]
        public async Task<IResult> KickMember([FromQuery] MemberData data)
        {
            try
            {
                if (!Guid.TryParse(data.groupId, out Guid groupGuid) || !Guid.TryParse(data.userId, out Guid userGuid))
                {
                    throw new BaseException("Invalid group ID or user ID format.", code: 400);
                }

                Guid coachGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var group = await _context.AthleteGroups
                    .Include(g => g.Athletes)
                    .FirstOrDefaultAsync(g => g.Id == groupGuid && g.CoachId == coachGuid);

                if (group == null)
                {
                    throw new BaseException("Group not found or you don't have permission to modify it.", code: 404);
                }

                var member = await _context.GroupMembers.FirstOrDefaultAsync(m => m.AthleteGroupId == groupGuid && m.AthleteId == userGuid);

                if (member == null)
                {
                    throw new BaseException("User is not a member of this group.", code: 404);
                }

                group.Athletes.Remove(member);
                _context.GroupMembers.Remove(member);

                await _context.SaveChangesAsync();

                return Results.Ok();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }


        [HttpPost("create"), Authorize(Roles = "coach")]
        public async Task<IResult> CreateGroup([FromBody] GroupModel group)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(group.OrganisationName))
                {
                    throw new BaseException("Organisation name is required.", code: 400);
                }

                Guid coachGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var newGroup = new AthleteGroup
                {
                    Id = Guid.NewGuid(),
                    CoachId = coachGuid,
                    OrganisationName = group.OrganisationName,
                    Athletes = new List<GroupMember>()
                };

                _context.AthleteGroups.Add(newGroup);

                await _context.SaveChangesAsync();

                return Results.Created();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }


        [HttpDelete("delete"), Authorize(Roles = "coach")]
        public async Task<IResult> DeleteGroup([FromQuery] string groupId)
        {
            try
            {
                if (!Guid.TryParse(groupId, out Guid groupGuid))
                {
                    return Results.BadRequest("Invalid group ID format.");
                }

                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var group = await _context.AthleteGroups
                    .Where(g => g.Id == groupGuid && g.CoachId == userGuid)
                    .FirstOrDefaultAsync();

                if (group == null)
                {
                    throw new BaseException("Group not found or you don't have permission to delete it.", code: 404);
                }

                _context.AthleteGroups.Remove(group);

                await _context.SaveChangesAsync();

                return Results.Ok();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }

        [HttpGet("unsubscribedathletes"), Authorize(Roles = "coach")]
        public async Task<IResult> GetUsersWithoutMembers([FromQuery] string groupId)
        {
            try
            {
                if (!Guid.TryParse(groupId, out Guid groupGuid))
                {
                    throw new BaseException("Invalid group ID format.", code: 400);
                }

                var users = await _context.Users
                    .Where(u => u.Role == "athlete")
                    .AsNoTracking()
                    .ToListAsync();

                var groupMembers = await _context.GroupMembers
                    .Where(gm => gm.AthleteGroupId == groupGuid)
                    .Select(gm => gm.AthleteId)
                    .ToListAsync();

                var filteredUsers = users
                    .Where(u => !groupMembers.Contains(u.Id))
                    .Select(FullUserModel.FromEntity)
                    .ToList();

                return Results.Json(filteredUsers);
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
    };
}