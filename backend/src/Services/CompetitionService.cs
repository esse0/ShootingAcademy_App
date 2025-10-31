using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Competition;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using System.Text.Json;

namespace ShootingAcademy.Services
{
    public interface ICompetitionService
    {
        Task<IEnumerable<CompetitionTypeResponse>> GetAllCompetitionsAsync();
        Task<CompetitionTypeResponse> GetCompetitionByIdAsync(string competitionId);
        Task<IEnumerable<CompetitionTypeResponse>> GetUserCompetitionsAsync(Guid userId, bool history = false);
        Task<IEnumerable<CompetitionTypeResponse>> GetOrganisatorCompetitionsAsync(Guid organisatorId);
        Task<CompetitionTypeResponse> CreateCompetitionAsync(CompetitionTypeResponse competition, Guid organisatorId);
        Task DeleteCompetitionAsync(string competitionId, Guid organisatorId);
        Task AddMemberToCompetitionAsync(string competitionId, string userId, Guid coachId);
        Task DeleteMemberFromCompetitionAsync(string competitionId, string userId, Guid coachId);
        Task<(byte[] content, string fileName)> ExportMembersAsync(string competitionId);
        Task ImportMembersAsync(string competitionId, CompetitionMemberResponse[] members, Guid organisatorId);
        Task ChangeCompetitionStatusAsync(string competitionId, string newStatus, Guid organisatorId);
        Task<IEnumerable<FullUserModel>> GetMyAthletesOutOfCompetitionAsync(string competitionId, Guid coachId);
        Task<IEnumerable<FullUserModel>> GetMyAthletesInTheCompetitionAsync(string competitionId, Guid coachId);
    }

    public class CompetitionService : ICompetitionService
    {
        private readonly ApplicationDbContext _context;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CompetitionService(ApplicationDbContext context)
        {
            _context = context;
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                IncludeFields = true
            };
        }

        public async Task<IEnumerable<CompetitionTypeResponse>> GetAllCompetitionsAsync()
        {
            var competitions = await _context.Competitions
                .Where(i => i.Status != Competition.ActiveStatus.Ended)
                .Include(c => c.Organisation)
                .Include(c => c.Members)
                .AsNoTracking()
                .ToListAsync();

            return competitions.Select(competition => new CompetitionTypeResponse
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
            });
        }

        public async Task<CompetitionTypeResponse> GetCompetitionByIdAsync(string competitionId)
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

            return new CompetitionTypeResponse
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
                members = competition.Members.Select(m => new CompetitionMemberResponse
                {
                    id = m.Id.ToString(),
                    fullName = $"{m.Athlete.FirstName} {m.Athlete.SecoundName}",
                    age = m.Athlete.Age,
                    country = m.Athlete.Country,
                    grade = m.Athlete.Grade,
                    result = m.Result
                }).ToList()
            };
        }

        public async Task<IEnumerable<CompetitionTypeResponse>> GetUserCompetitionsAsync(Guid userId, bool history = false)
        {
            var user = await _context.Users
                .Include(u => u.Competitions)
                .ThenInclude(tc => tc.Competition)
                .ThenInclude(c => c.Organisation)
                .Include(u => u.Competitions)
                .ThenInclude(tc => tc.Competition.Members)
                .FirstAsync(u => u.Id == userId);

            return user.Competitions
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
        }

        public async Task<IEnumerable<CompetitionTypeResponse>> GetOrganisatorCompetitionsAsync(Guid organisatorId)
        {
            var competitions = await _context.Competitions
                .Where(c => c.OrganisationId == organisatorId)
                .Include(c => c.Organisation)
                .Include(c => c.Members)
                .ToListAsync();

            return competitions.Select(c => new CompetitionTypeResponse
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
        }

        public async Task<CompetitionTypeResponse> CreateCompetitionAsync(CompetitionTypeResponse competition, Guid organisatorId)
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

            var organiser = await _context.Users.FirstOrDefaultAsync(u => u.Id == organisatorId);
            if (organiser == null)
                throw new BaseException("Organizer not found.", 404);

            var newCompetition = new Competition
            {
                Title = competition.title,
                Description = competition.description,
                DateTime = competitionDateTime,
                MaxMembersCount = competition.maxMemberCount,
                Venue = competition.venue,
                Country = competition.country,
                City = competition.city,
                Exercise = competition.exercise,
                Status = Competition.ActiveStatus.Pending,
                OrganisationId = organisatorId
            };

            await _context.Competitions.AddAsync(newCompetition);
            await _context.SaveChangesAsync();

            competition.id = newCompetition.Id.ToString();
            competition.status = Enum.GetName(typeof(Competition.ActiveStatus), newCompetition.Status);
            competition.organiser = $"{organiser.FirstName} {organiser.SecoundName}";
            competition.memberCount = 0;

            return competition;
        }

        public async Task DeleteCompetitionAsync(string competitionId, Guid organisatorId)
        {
            if (!Guid.TryParse(competitionId, out Guid competitionGuid))
            {
                throw new BaseException("Invalid competition ID format.", 400);
            }

            var competition = await _context.Competitions
                .FirstOrDefaultAsync(c => c.Id == competitionGuid && c.OrganisationId == organisatorId);

            if (competition == null)
            {
                throw new BaseException("Competition not found or you don't have permission to delete it.", 404);
            }

            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();
        }

        public async Task AddMemberToCompetitionAsync(string competitionId, string userId, Guid coachId)
        {
            if (!Guid.TryParse(competitionId, out Guid competitionGuid) || !Guid.TryParse(userId, out Guid userGuid))
            {
                throw new BaseException("Invalid ID format.", 400);
            }

            var competition = await _context.Competitions
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == competitionGuid);

            if (competition == null)
            {
                throw new BaseException("Competition not found.", 404);
            }

            if (competition.Members.Count >= competition.MaxMembersCount)
            {
                throw new BaseException("Competition is full.", 400);
            }

            var athlete = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userGuid);

            if (athlete == null)
            {
                throw new BaseException("Athlete not found.", 404);
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

            if (competition.Members.Any(m => m.AthleteId == userGuid))
            {
                throw new BaseException("Athlete is already in the competition.", 400);
            }

            var member = new CompetitionMember
            {
                CompetitionId = competitionGuid,
                AthleteId = userGuid
            };

            await _context.CompetitionMembers.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMemberFromCompetitionAsync(string competitionId, string userId, Guid coachId)
        {
            if (!Guid.TryParse(competitionId, out Guid competitionGuid) || !Guid.TryParse(userId, out Guid userGuid))
            {
                throw new BaseException("Invalid ID format.", 400);
            }

            if (!await _context.Users.AnyAsync(u => u.Id == userGuid))
                throw new BaseException("User not found!", 404);

            var competition = await _context.Competitions
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == competitionGuid)
                ?? throw new BaseException("Competition don`t found!", 404);

            var commem = competition.Members.FirstOrDefault(m => m.AthleteId == userGuid && m.CompetitionId == competitionGuid)
                ?? throw new BaseException("User not participate in this competition!");

            _context.CompetitionMembers.Remove(commem);

            await _context.SaveChangesAsync();
        }

        public async Task<(byte[] content, string fileName)> ExportMembersAsync(string competitionId)
        {
            if (!Guid.TryParse(competitionId, out Guid competitionGuid))
                throw new BaseException("Invalid competition ID format.", 400);

            var competition = await _context.Competitions
                .Include(c => c.Members)
                .ThenInclude(m => m.Athlete)
                .FirstOrDefaultAsync(c => c.Id == competitionGuid);

            if (competition == null)
                throw new BaseException("Competition not found.", 404);

            var members = competition.Members.Select(m => new
            {
                id = m.AthleteId,
                fullName = $"{m.Athlete.FirstName} {m.Athlete.SecoundName}",
                age = m.Athlete.Age,
                country = m.Athlete.Country,
                grade = m.Athlete.Grade,
                result = m.Result
            }).ToList();

            var json = JsonSerializer.Serialize(members, _jsonSerializerOptions);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var fileName = $"competition_members_{competitionId}.json";

            return (bytes, fileName);
        }

        public async Task ImportMembersAsync(string competitionId, CompetitionMemberResponse[] members, Guid organisatorId)
        {
            if (!Guid.TryParse(competitionId, out Guid competitionGuid))
            {
                throw new BaseException("Invalid competition ID format.", 400);
            }

            var competition = await _context.Competitions
                .Include(comp => comp.Members)
                .FirstOrDefaultAsync(c => c.Id == competitionGuid && c.OrganisationId == organisatorId);

            if (competition == null)
            {
                throw new BaseException("Competition not found or you don't have permission to modify it.", 404);
            }

            foreach (var member in members)
            {
                if (!Guid.TryParse(member.id, out Guid athleteGuid))
                {
                    throw new BaseException($"Invalid athlete ID format for member.", 400);
                }

                var athlete = await _context.Users.FindAsync(athleteGuid);
                
                if (athlete == null)
                    continue;

                var competitionMember = competition.Members.Find(el => el.AthleteId == athleteGuid);

                if (competitionMember != null)
                {
                    competitionMember.Result = member.result;
                }
            }


            await _context.SaveChangesAsync();
        }


        //public async Task ImportMembersAsync(string competitionId, CompetitionMemberResponse[] members, Guid organisatorId)
        //{
        //    if (!Guid.TryParse(competitionId, out Guid competitionGuid))
        //    {
        //        throw new BaseException("Invalid competition ID format.", 400);
        //    }

        //    var competition = await _context.Competitions
        //        .Include(c => c.Members)
        //        .FirstOrDefaultAsync(c => c.Id == competitionGuid && c.OrganisationId == organisatorId);

        //    if (competition == null)
        //    {
        //        throw new BaseException("Competition not found or you don't have permission to modify it.", 404);
        //    }

        //    foreach (var member in members)
        //    {
        //        if (string.IsNullOrWhiteSpace(member.fullName) || member.age <= 0)
        //        {
        //            throw new BaseException("Invalid member data.", 400);
        //        }

        //        var nameParts = member.fullName.Split(' ');
        //        if (nameParts.Length < 2)
        //        {
        //            throw new BaseException("Invalid name format. Full name should contain first and last name.", 400);
        //        }

        //        var athlete = new User
        //        {
        //            FirstName = nameParts[0],
        //            SecoundName = string.Join(" ", nameParts.Skip(1)) ?? "",
        //            Age = member.age,
        //            Country = member.country,
        //            Grade = member.grade,
        //            Role = "athlete"
        //        };

        //        await _context.Users.AddAsync(athlete);
        //        await _context.SaveChangesAsync();

        //        var competitionMember = new CompetitionMember
        //        {
        //            CompetitionId = competitionGuid,
        //            AthleteId = athlete.Id,
        //            Result = member.result
        //        };

        //        await _context.CompetitionMembers.AddAsync(competitionMember);
        //    }

        //    await _context.SaveChangesAsync();
        //}

        public async Task ChangeCompetitionStatusAsync(string competitionId, string newStatus, Guid organisatorId)
        {
            if (!Guid.TryParse(competitionId, out Guid competitionGuid))
            {
                throw new BaseException("Invalid competition ID format.", 400);
            }

            if (!Enum.TryParse<Competition.ActiveStatus>(newStatus, out var status))
            {
                throw new BaseException("Invalid status value.", 400);
            }

            var competition = await _context.Competitions
                .FirstOrDefaultAsync(c => c.Id == competitionGuid && c.OrganisationId == organisatorId);

            if (competition == null)
            {
                throw new BaseException("Competition not found or you don't have permission to modify it.", 404);
            }

            competition.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FullUserModel>> GetMyAthletesOutOfCompetitionAsync(string competitionId, Guid coachId)
        {
            if (!Guid.TryParse(competitionId, out Guid competitionGuid))
            {
                throw new BaseException("Invalid competition ID format.", 400);
            }

            var competition = await _context.Competitions
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == competitionGuid);

            if (competition == null)
            {
                throw new BaseException("Competition not found.", 404);
            }

            var athletesInCompetition = competition.Members.Select(m => m.AthleteId).ToList();

            var athleteGroups = await _context.AthleteGroups
                   .Where(g => g.CoachId == coachId)
                   .Include(g => g.Athletes)
                   .ThenInclude(g => g.Athlete)
                   .ToListAsync();

            if (athleteGroups == null || athleteGroups.Count == 0)
                throw new BaseException("Coach's athlete groups not found", code: 404);


            List<User> athletes = [];
            foreach (var groups in athleteGroups)
            {
                foreach (var athlete in groups.Athletes)
                {
                    if (athlete.Status != GroupMemberStatus.Approved) continue;
                    if (!athletes.Contains(athlete.Athlete))
                        athletes.Add(athlete.Athlete);
                }
            }

            var myAthletes = athletes
                .Where(ath => !athletesInCompetition.Contains(ath.Id));

            return myAthletes.Select(FullUserModel.FromEntity);
        }

        public async Task<IEnumerable<FullUserModel>> GetMyAthletesInTheCompetitionAsync(string competitionId, Guid coachId)
        {
            if (!Guid.TryParse(competitionId, out Guid competitionGuid))
            {
                throw new BaseException("Invalid competition ID format.", 400);
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

            return noncompAthletes;
        }
    }
} 