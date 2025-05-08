using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models;
using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Services;
using ShootingAcademy.Models.Controllers.Competition;
using System.Text;
using System.Text.Json;

namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CompetitionController(ApplicationDbContext context)
        {
            _context = context;

            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                IncludeFields = true
            };
        }

        [HttpGet]
        public async Task<IResult> Get()
        {
            IEnumerable<Competition> Competitions = await _context.Competitions
                .Where(i => i.Status != Competition.ActiveStatus.Ended)
                .Include(c => c.Organisation)
                .Include(c => c.Members)
                .AsNoTracking()
                .ToListAsync();

            return Results.Json(Competitions.Select(competition =>
            {
                return new CompetitionTypeResponse
                {
                    status = Enum.GetName(typeof(Competition.ActiveStatus), competition.Status),
                    city = competition.City,
                    country = competition.Country,
                    maxMemberCount = competition.MaxMembersCount,
                    memberCount = competition.Members?.Count ?? 0,
                    date = competition.DateTime.ToUniversalTime().ToString("yyyy-MM-dd"),
                    time = competition.DateTime.ToUniversalTime().ToString("HH:mm"),
                    description = competition.Description,
                    exercise = competition.Exercise,
                    id = competition.Id.ToString(),
                    organiser = competition.Organisation != null
                        ? $"{competition.Organisation.FirstName} {competition.Organisation.SecoundName}"
                        : "Unknown",
                    title = competition.Title,
                    venue = competition.Venue,
                };
            }));
        }

        [HttpGet("fulldata")]
        public async Task<IResult> GetCompetitionById([FromQuery] string competitionId)
        {
            try
            {
                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                {
                    throw new BaseException("Invalid competition ID format.", code: 400);
                }

                var competition = await _context.Competitions
                    .Include(c => c.Organisation)
                    .Include(c => c.Members)   
                    .ThenInclude(m => m.Athlete)
                    .FirstOrDefaultAsync(c => c.Id == competitionGuid);

                if (competition == null)
                {
                    throw new BaseException("Competition not found", 404);
                }

                var competitionDetails = new CompetitionTypeResponse
                {
                    id = competition.Id.ToString(),
                    title = competition.Title,
                    description = competition.Description,
                    date = competition.DateTime.ToUniversalTime().ToString("yyyy-MM-dd"),
                    time = competition.DateTime.ToUniversalTime().ToString("HH:mm"),
                    maxMemberCount = competition.MaxMembersCount,
                    memberCount = competition.Members.Count,
                    venue = competition.Venue,
                    country = competition.Country,
                    city = competition.City,
                    exercise = competition.Exercise,
                    status = Enum.GetName(typeof(Competition.ActiveStatus), competition.Status),
                    organiser = competition.Organisation != null
                        ? $"{competition.Organisation.FirstName} {competition.Organisation.SecoundName}"
                        : "Unknown",
                    members = competition.Members.Select(m => new Models.Controllers.Competition.CompetitionMemberResponse
                    {
                        id = m.Id.ToString(),
                        fullName = $"{m.Athlete.FirstName} {m.Athlete.SecoundName}",
                        age = m.Athlete.Age,
                        country = m.Athlete.Country,
                        grade = m.Athlete.Grade,
                        result = m.Result
                    }).ToList()
                };

                return Results.Json(competitionDetails);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
        }


        [HttpGet("user"), Authorize]
        public async Task<IResult> GetUserCompetitions([FromQuery] bool history = false)
        {
            try
            {
                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                User user = await _context.Users
                    .Include(u => u.Competitions)
                    .ThenInclude(tc => tc.Competition)
                    .ThenInclude(c => c.Organisation)
                    .Include(u => u.Competitions)
                    .ThenInclude(tc => tc.Competition.Members)
                    .FirstAsync(u => u.Id == userGuid);

                var competitionTypes = user.Competitions
                    .Select(tc => tc.Competition)
                    .Where(competition => history ? competition.Status == Competition.ActiveStatus.Ended
                                                : competition.Status != Competition.ActiveStatus.Ended)
                    .Select(competition => new CompetitionTypeResponse
                    {
                        status = Enum.GetName(typeof(Competition.ActiveStatus), competition.Status),
                        city = competition.City,
                        country = competition.Country,
                        maxMemberCount = competition.MaxMembersCount,
                        memberCount = competition.Members.Count,
                        date = competition.DateTime.ToUniversalTime().ToString("yyyy-MM-dd"),
                        time = competition.DateTime.ToUniversalTime().ToString("HH:mm"),
                        description = competition.Description,
                        exercise = competition.Exercise,
                        id = competition.Id.ToString(),
                        organiser = $"{competition.Organisation.FirstName} {competition.Organisation.SecoundName}",
                        title = competition.Title,
                        venue = competition.Venue
                    })
                    .ToList();

                return Results.Json(competitionTypes);
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


        [HttpGet("organisator"), Authorize(Roles = "organisator")]
        public async Task<IResult> GetOrganisatorCompetitions()
        {
            try
            {
                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var competitions = await _context.Competitions
                    .Where(c => c.OrganisationId == userGuid)
                    .Include(c => c.Organisation)
                    .Include(c => c.Members)
                    .AsNoTracking()
                    .ToListAsync();

                var competitionTypes = competitions.Select(c => new CompetitionTypeResponse
                {
                    id = c.Id.ToString(),
                    title = c.Title,
                    description = c.Description,
                    date = c.DateTime.ToUniversalTime().ToString("yyyy-MM-dd"),
                    time = c.DateTime.ToUniversalTime().ToString("HH:mm"),
                    maxMemberCount = c.MaxMembersCount,
                    memberCount = c.Members.Count,
                    venue = c.Venue,
                    country = c.Country,
                    city = c.City,
                    exercise = c.Exercise,
                    status = Enum.GetName(typeof(Competition.ActiveStatus), c.Status),
                    organiser = $"{c.Organisation.FirstName} {c.Organisation.SecoundName}"
                }).ToList();

                return Results.Json(competitionTypes);
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

        [HttpPost("create"), Authorize(Roles = "organisator")]
        public async Task<IResult> CreateCompetition([FromBody] CompetitionTypeResponse competition)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(competition.title) ||
                    string.IsNullOrWhiteSpace(competition.description) ||
                    string.IsNullOrWhiteSpace(competition.date) ||
                    string.IsNullOrWhiteSpace(competition.time))
                {
                    throw new BaseException("Invalid input: required fields are missing.", 400);
                }

                if (competition.maxMemberCount <= 0)
                    throw new BaseException($"Max member count must be greater than 0.", 400);


                if (!DateTime.TryParse(competition.date, out DateTime competitionDate))
                    throw new BaseException("Invalid date or date format.", 400);

                if (!DateTime.TryParse(competition.time, out DateTime competitionTime))
                    throw new BaseException("Invalid time format.", 400);

                var competitionDateTime = competitionDate.Add(competitionTime.TimeOfDay);

                competitionDateTime = competitionDateTime.ToUniversalTime();

                Guid organiserGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var organiser = await _context.Users.FirstOrDefaultAsync(u => u.Id == organiserGuid);

                if (organiser == null)
                    throw new BaseException("Organizer not found.", 404);

                var newCompetition = new Competition
                {
                    Id = Guid.NewGuid(),
                    Title = competition.title,
                    Description = competition.description,
                    DateTime = competitionDateTime,
                    MaxMembersCount = competition.maxMemberCount,
                    Venue = competition.venue,
                    City = competition.city,
                    Country = competition.country,
                    Status = Competition.ActiveStatus.Pending,
                    Exercise = competition.exercise,
                    OrganisationId = organiserGuid
                };

                _context.Competitions.Add(newCompetition);

                await _context.SaveChangesAsync();

                return Results.StatusCode(201);
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

        [HttpDelete("delete"), Authorize(Roles = "organisator")]
        public async Task<IResult> DeleteCompetition([FromQuery] string competitionId)
        {
            try
            {
                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                {
                    throw new BaseException("Invalid competition ID format.", code: 400);
                }

                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var competition = await _context.Competitions
                .FirstOrDefaultAsync(c => c.Id == competitionGuid);

                if (competition == null)
                {
                    throw new BaseException("Competition not found.", code: 404);
                }

                if (competition.OrganisationId != userGuid)
                {
                    throw new BaseException("Unauthorized", code: 401);
                }

                _context.Competitions.Remove(competition);

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

        [HttpPost("addmember"), Authorize(Roles = "coach")]
        public async Task<IResult> AddMemberCompetition([FromQuery] string competitionId, [FromQuery] string userId)
        {
            try
            {
                Guid coachId = AutorizeData.FromContext(HttpContext).UserGuid;

                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                {
                    throw new BaseException("Invalid competition ID format.", code: 400);
                }

                if (!Guid.TryParse(userId, out Guid userGuid))
                {
                    throw new BaseException("Invalid user ID format.", code: 400);
                }

                var user = await _context.Users.FindAsync(userGuid);

                if (user == null)
                {
                    throw new BaseException("User not found", code: 404);
                }

                var competition = await _context.Competitions
                   .Include(c => c.Members)
                   .FirstOrDefaultAsync(c => c.Id == competitionGuid);

                if (competition == null)
                {
                    throw new BaseException("Competition not found", 404);
                }

                if (competition.Members.Count == competition.MaxMembersCount)
                {
                    throw new BaseException("Competition is full", 400);
                }

                var athleteGroups = await _context.AthleteGroups
                    .Include(g => g.Athletes)
                    .Where(g => g.CoachId == coachId)
                    .ToListAsync();

                var userIsInAnyGroup = athleteGroups
                    .Any(g => g.Athletes.Any(a => a.AthleteId == userGuid));

                if (!userIsInAnyGroup)
                {
                    throw new BaseException("User is not part of any of your groups", 400);
                }

                var userAlreadyInCompetition = competition.Members.Any(m => m.AthleteId.ToString() == userId);

                if (userAlreadyInCompetition)
                {
                    throw new BaseException("User is already part of the competition", 400);
                }

                var competitionMember = new CompetitionMember
                {
                    CompetitionId = competitionGuid,
                    AthleteId = user.Id,
                    Result = null
                };

                competition.Members.Add(competitionMember);

                await _context.SaveChangesAsync();

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
        }

        
        [HttpDelete("deletemember"), Authorize(Roles = "coach")]
        public async Task<IResult> DeleteMemberCompetition([FromQuery] string competitionId, [FromQuery] string userId)
        {
            try
            {
                var coachId = AutorizeData.FromContext(HttpContext).UserGuid;

                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                    throw new BaseException("Competition id is not correct!");

                if (!Guid.TryParse(userId, out Guid userGuid))
                    throw new BaseException("User id is not correct!");

                if (!await _context.Users.AnyAsync(u => u.Id == userGuid))
                    throw new BaseException("User don`t found!", 404);

                var competition = await _context.Competitions
                    .Include(c => c.Members)
                    .FirstOrDefaultAsync(c => c.Id == competitionGuid)
                    ?? throw new BaseException("Competition don`t found!", 404);

                var commem = competition.Members.FirstOrDefault(m => m.AthleteId == userGuid && m.CompetitionId == competitionGuid)
                    ?? throw new BaseException("User not participate in this competition!");

                _context.CompetitionMembers.Remove(commem);

                await _context.SaveChangesAsync();

                return Results.Ok();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
        }

        [HttpGet("export"), Authorize]
        public async Task<IResult> ExportMembers([FromQuery] string competitionId)
        {
            try
            {
                Guid userId = AutorizeData.FromContext(HttpContext).UserGuid;

                var user = await _context.Users.FindAsync(userId);

                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                    throw new BaseException("Competition id is not correct!");

                var competion = await _context.Competitions
                    .Include(c => c.Members)
                    .ThenInclude(c => c.Athlete)
                    .FirstOrDefaultAsync(c => c.Id == competitionGuid)
                    ?? throw new BaseException("Competition not be found!", 404);

                if (competion.OrganisationId != userId)
                    throw new BaseException("Competition is not yours!", 403);

                if (competion.Members.Count <= 0)
                    throw new BaseException("Competition have zero members!");

                var members = competion.Members.Select(m =>
                {
                    return new CompetitionMemberResponse()
                    {
                        age = m.Athlete.Age,
                        fullName = $"{m.Athlete.FirstName} {m.Athlete.SecoundName}",
                        country = m.Athlete.Country,
                        grade = m.Athlete.Grade,
                        id = m.Athlete.Id.ToString(),
                        result = m.Result
                    };
                });

                string json = JsonSerializer.Serialize(members, _jsonSerializerOptions);

                return Results.File(Encoding.UTF8.GetBytes(json), "application/json", fileDownloadName: $"{competion.Title}_Members.json");
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
        }

        [HttpPost("import"), Authorize(Roles = "organisator")]
        public async Task<IResult> ImportMembers([FromQuery] string competitionId, [FromBody] CompetitionMemberResponse[] members)
        {
            try
            {
                Guid userId = AutorizeData.FromContext(HttpContext).UserGuid;

                var user = await _context.Users.FindAsync(userId);

                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                    throw new BaseException("Competition id is not correct!");

                var competion = await _context.Competitions
                    .Include(c => c.Members)
                    .FirstOrDefaultAsync(c => c.Id == competitionGuid)
                    ?? throw new BaseException("Competition not be found!", 404);

                if (competion.OrganisationId != userId)
                    throw new BaseException("Competition is not yours!", 403);

                foreach (var member in members)
                {
                    if (!Guid.TryParse(member.id, out Guid memberId))
                        throw new BaseException("One of users have worng id!");

                    if (!_context.Users.Any(u => u.Id == memberId))
                        throw new BaseException("One of users doesn`t exist!");

                    var commem = competion.Members.FirstOrDefault(m => m.AthleteId == memberId);

                    if (commem != null)
                    {
                        commem.Result = member.result;

                        _context.CompetitionMembers.Update(commem);
                    } 
                    else
                    {
                        await _context.AddAsync(new CompetitionMember()
                        {
                            AthleteId = Guid.Parse(member.id),
                            CompetitionId = competion.Id,
                            Result = member.result
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return Results.Ok();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
        }

        [HttpPut("status"), Authorize(Roles = "organisator")]
        public async Task<IResult> ChangeStatus([FromQuery] string competitionId, [FromQuery] string newStatus)
        {
            try
            {
                Guid userId = AutorizeData.FromContext(HttpContext).UserGuid;

                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                    throw new BaseException("Competition id is not correct!");

                var competition = await _context.Competitions.FindAsync(competitionGuid)
                    ?? throw new BaseException("Competition not found!", 404);

                if (competition.OrganisationId != userId)
                    throw new BaseException("Competition is not yours!", 403);

                if (!Enum.TryParse<Competition.ActiveStatus>(newStatus, out var status))
                    throw new BaseException("Status can be only: (Pending, Active, Ended)");

                competition.Status = status;

                _context.Competitions.Update(competition);

                await _context.SaveChangesAsync();

                return Results.Ok();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
        }


        [HttpGet("myathletesoutofcompetition"), Authorize(Roles = "coach")]
        public async Task<IResult> GetMyAthletesOutOfCompetition([FromQuery] string competitionId)
        {
            try
            {
                // Получаем ID тренера
                Guid coachId = AutorizeData.FromContext(HttpContext).UserGuid;

                // Проверка на правильность формата ID соревнования
                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                {
                    throw new BaseException("Bad id", code: 400);
                }

                // Получаем информацию о соревновании
                var competition = await _context.Competitions
                    .Include(c => c.Members)
                    .FirstOrDefaultAsync(c => c.Id == competitionGuid);

                if (competition == null)
                {
                    throw new BaseException("Competition not found", code: 404);
                }

                // Проверка, что список членов соревнования не null
                if (competition.Members == null)
                {
                    competition.Members = new List<CompetitionMember>(); // Инициализация пустого списка, если он null
                }

                // Получаем все группы атлетов тренера
                var athleteGroups = await _context.AthleteGroups
                    .Where(g => g.CoachId == coachId)
                    .Include(g => g.Athletes)
                    .ThenInclude(g => g.Athlete)
                    .ToListAsync();

                // Проверка на наличие групп атлетов
                if (athleteGroups == null || !athleteGroups.Any())
                    throw new BaseException("Coach's athlete groups not found", code: 404);

                List<User> athletes = [];
                foreach (var groups in athleteGroups)
                {
                    foreach (var athlete in groups.Athletes)
                    {
                        if (!athletes.Contains(athlete.Athlete))
                            athletes.Add(athlete.Athlete);
                    }
                }

                var noncompAthletes = athletes
                    .Where(ath => !competition.Members.Any(m => m.AthleteId == ath.Id))
                    .Select(FullUserModel.FromEntity);

                return Results.Json(noncompAthletes);
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr. Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 400);
            }
        }

        [HttpGet("myathletesinthecompetition"), Authorize(Roles = "coach")]
        public async Task<IResult> GetMyAthletesInTheCompetition([FromQuery] string competitionId)
        {
            try
            {
                Guid coachId = AutorizeData.FromContext(HttpContext).UserGuid;

                if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                {
                    throw new BaseException("Bad id", code: 400);
                }

                var competition = await _context.Competitions
                    .Include(c => c.Members)
                    .FirstOrDefaultAsync(c => c.Id == competitionGuid);

                if (competition == null)
                {
                    throw new BaseException("Competition not found", code: 404);
                }

                var athleteGroups = await _context.AthleteGroups
                    .Where(g => g.CoachId == coachId)
                    .Include(g => g.Athletes)
                    .ThenInclude(g => g.Athlete)
                    .ToListAsync();

                if (athleteGroups == null || !athleteGroups.Any())
                {
                    throw new BaseException("Coach's athlete groups not found", code: 404);
                }

                List<User> athletes = [];
                foreach (var groups in athleteGroups)
                {
                    foreach (var athlete in groups.Athletes)
                    {
                        if (!athletes.Contains(athlete.Athlete))
                            athletes.Add(athlete.Athlete);
                    }
                }

                var noncompAthletes = athletes
                    .Where(ath => competition.Members.Any(m => m.AthleteId == ath.Id))
                    .Select(FullUserModel.FromEntity);

                return Results.Json(noncompAthletes);
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
