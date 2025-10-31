using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.AthleteGroup;
using ShootingAcademy.Models.Controllers.Group;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services.Media;

namespace ShootingAcademy.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<AthleteGroupModel>> GetAllGroupsAsync();
        Task<IEnumerable<AthleteGroupModel>> GetUserGroupsAsync(Guid userId);
        Task<IEnumerable<AthleteGroupModel>> GetCoachGroupsAsync(Guid coachId);
        Task<AthleteGroupModel> GetUserGroupAsync(string groupId, Guid userId);
        Task<AthleteGroupModel> GetCoachGroupAsync(string groupId, Guid coachId);
        Task InviteMemberAsync(Guid groupId, Guid athleteId, Guid coachId);
        Task KickMemberAsync(string groupId, string athleteId, Guid coachId);
        Task CreateGroupAsync(GroupModel group, Guid coachId);
        Task DeleteGroupAsync(string groupId, Guid coachId);
        Task UpdateStatusMemberAsync(Guid groupId, Guid athleteId, Guid coachId, GroupMemberStatus status);
        Task<IEnumerable<AthleteWithStatusDTO>> GetUnsubscribedAthletesAsync(string groupId, Guid coachId);
        Task<AthleteGroupModel> GetGroupByIdAsync(Guid groupId);
    }

    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public GroupService(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<AthleteGroupModel> GetGroupByIdAsync(Guid groupId)
        {
            var athleteGroup = await _context.AthleteGroups
               .Include(g => g.Coach)
               .FirstOrDefaultAsync(group => group.Id == groupId);

            if (athleteGroup == null)
            {
                throw new BaseException("Group not found.", code: 404);
            }

            return new AthleteGroupModel
            {
                id = athleteGroup.Id.ToString(),
                organisationName = athleteGroup.OrganisationName,
                coach = UserWithAvatar.FromEntity(athleteGroup.Coach, "")
            };
        }

        public async Task<IEnumerable<AthleteGroupModel>> GetAllGroupsAsync()
        {
            var athleteGroups = await _context.AthleteGroups
                .Include(g => g.Coach)
                .AsNoTracking()
                .ToListAsync();

            return athleteGroups.Select(group => new AthleteGroupModel
            {
                id = group.Id.ToString(),
                organisationName = group.OrganisationName,
                coach = UserWithAvatar.FromEntity(group.Coach, "")
            });
        }

        public async Task<IEnumerable<AthleteGroupModel>> GetUserGroupsAsync(Guid userId)
        {
            var athleteGroups = await _context.AthleteGroups
                .Include(g => g.Coach)
                .Include(g => g.Athletes)
                .Where(g => g.Athletes.Any(a => a.AthleteId == userId && a.Status == GroupMemberStatus.Approved))
                .AsNoTracking()
                .ToListAsync();

            return athleteGroups.Select(group => new AthleteGroupModel
            {
                id = group.Id.ToString(),
                organisationName = group.OrganisationName,
                coach = UserWithAvatar.FromEntity(group.Coach, "")
            });
        }

        public async Task<IEnumerable<AthleteGroupModel>> GetCoachGroupsAsync(Guid coachId)
        {
            var athleteGroups = await _context.AthleteGroups
                .Where(g => g.CoachId == coachId)
                .Include(g => g.Coach)
                .AsNoTracking()
                .ToListAsync();

            return athleteGroups.Select(group => new AthleteGroupModel
            {
                id = group.Id.ToString(),
                organisationName = group.OrganisationName,
                coach = UserWithAvatar.FromEntity(group.Coach, "")
            });
        }

        public async Task<AthleteGroupModel> GetUserGroupAsync(string groupId, Guid userId)
        {
            if (!Guid.TryParse(groupId, out Guid groupGuid))
            {
                throw new BaseException("Invalid group ID format.", code: 400);
            }

            var group = await _context.AthleteGroups
                .Where(g => g.Id == groupGuid)
                .Include(g => g.Coach)
                .Include(g => g.Athletes)
                .ThenInclude(a => a.Athlete)
                .Where(g => g.Athletes.Any(a => a.AthleteId == userId && a.Status == GroupMemberStatus.Approved))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (group == null)
            {
                throw new BaseException("Group not found.", code: 404);
            }

            var coachPhoto = await _imageService.GetFileUrl(group.CoachId);

            return new AthleteGroupModel
            {
                id = group.Id.ToString(),
                organisationName = group.OrganisationName,
                coach = UserWithAvatar.FromEntity(group.Coach, coachPhoto.FileUri),
                members = group.Athletes?
                    .Where(a => a.Athlete != null && a.Status == GroupMemberStatus.Approved)
                    .Select(a => FullUserModel.FromEntity(a.Athlete))
                    .ToList() ?? new List<FullUserModel>()
            };
        }

        public async Task<AthleteGroupModel> GetCoachGroupAsync(string groupId, Guid coachId)
        {
            if (!Guid.TryParse(groupId, out Guid groupGuid))
            {
                throw new BaseException("Invalid group ID format.", code: 400);
            }

            var group = await _context.AthleteGroups
                .Where(g => g.Id == groupGuid && g.CoachId == coachId)
                .Include(g => g.Coach)
                .Include(g => g.Athletes)
                .ThenInclude(a => a.Athlete)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (group == null)
            {
                throw new BaseException("Group not found.", code: 404);
            }

            var coachPhoto = await _imageService.GetFileUrl(group.CoachId);

            return new AthleteGroupModel
            {
                id = group.Id.ToString(),
                organisationName = group.OrganisationName,
                coach = UserWithAvatar.FromEntity(group.Coach, coachPhoto.FileUri),
                members = group.Athletes?
                    .Where(a => a.Athlete != null && a.Status == GroupMemberStatus.Approved)
                    .Select(a => FullUserModel.FromEntity(a.Athlete))
                    .ToList() ?? new List<FullUserModel>()
            };
        }

        public async Task InviteMemberAsync(Guid groupId, Guid athleteId, Guid coachId)
        {
            var group = await _context.AthleteGroups
                .Include(group => group.Athletes)
                .FirstOrDefaultAsync(g => g.Id == groupId && g.CoachId == coachId);

            if (group == null)
            {
                throw new BaseException("Group not found.", code: 404);
            }

            var athlete = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == athleteId && u.Role == "athlete");

            if (athlete == null)
            {
                throw new BaseException("Athlete not found.", code: 404);
            }

            var userInGroup = group.Athletes.Find(a => a.AthleteId == athleteId);

            if(userInGroup != null)
            {
                if (userInGroup.Status == GroupMemberStatus.Pending)
                {
                    throw new BaseException("Athlete invite already sended", code: 400);
                }
                else
                {
                    await UpdateStatusMemberAsync(groupId, athleteId, coachId, GroupMemberStatus.Pending);
                    return;
                }
            }

            _context.GroupMembers.Add(new GroupMember
            {
                Id = Guid.NewGuid(),
                AthleteGroupId = groupId,
                AthleteId = athleteId,
                Status = GroupMemberStatus.Pending,
            });

            await _context.SaveChangesAsync();

        }

        public async Task UpdateStatusMemberAsync(Guid groupId, Guid athleteId, Guid coachId, GroupMemberStatus status)
        {
            var group = await _context.AthleteGroups
                 .FirstOrDefaultAsync(g => g.Id == groupId && g.CoachId == coachId);

            if (group == null)
            {
                throw new BaseException("Group not found.", code: 404);
            }

            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.AthleteGroupId == groupId && m.AthleteId == athleteId);

            if (member == null)
            {
                throw new BaseException("Athlete is not a member of this group.", code: 404);
            }

            member.Status = status;

            await _context.SaveChangesAsync();
        }

        public async Task KickMemberAsync(string groupId, string athleteId, Guid coachId)
        {
            if (!Guid.TryParse(groupId, out Guid groupGuid) || !Guid.TryParse(athleteId, out Guid athleteGuid))
            {
                throw new BaseException("Invalid ID format.", code: 400);
            }

            var group = await _context.AthleteGroups
                .FirstOrDefaultAsync(g => g.Id == groupGuid && g.CoachId == coachId);

            if (group == null)
            {
                throw new BaseException("Group not found.", code: 404);
            }

            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.AthleteGroupId == groupGuid && m.AthleteId == athleteGuid);

            if (member == null)
            {
                throw new BaseException("Athlete is not a member of this group.", code: 404);
            }
            member.Status = GroupMemberStatus.Removed;

            await _context.SaveChangesAsync();
        }

        public async Task CreateGroupAsync(GroupModel group, Guid coachId)
        {
            if (string.IsNullOrWhiteSpace(group.OrganisationName))
            {
                throw new BaseException("Organization name is required.", code: 400);
            }

            var newGroup = new AthleteGroup
            {
                Id = Guid.NewGuid(),
                OrganisationName = group.OrganisationName,
                CoachId = coachId
            };

            _context.AthleteGroups.Add(newGroup);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(string groupId, Guid coachId)
        {
            if (!Guid.TryParse(groupId, out Guid groupGuid))
            {
                throw new BaseException("Invalid group ID format.", code: 400);
            }

            var group = await _context.AthleteGroups
                .FirstOrDefaultAsync(g => g.Id == groupGuid && g.CoachId == coachId);

            if (group == null)
            {
                throw new BaseException("Group not found.", code: 404);
            }

            _context.AthleteGroups.Remove(group);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AthleteWithStatusDTO>> GetUnsubscribedAthletesAsync(string groupId, Guid coachId)
        {
            if (!Guid.TryParse(groupId, out Guid groupGuid))
            {
                throw new BaseException("Invalid group ID format.", code: 400);
            }

            var group = await _context.AthleteGroups
                .Include(group => group.Athletes)
                .FirstOrDefaultAsync(g => g.Id == groupGuid && g.CoachId == coachId);

            if (group == null)
            {
                throw new BaseException("Group not found.", code: 404);
            }

            var allAthletes = await _context.Users
                .Where(u => u.Role == "athlete")
                .AsNoTracking()
                .ToListAsync();

            var result = new List<AthleteWithStatusDTO>();

            foreach (var athlete in allAthletes)
            {
                var member = group.Athletes.FirstOrDefault(a => a.AthleteId == athlete.Id);
                
                if (member == null || member.Status != GroupMemberStatus.Approved)
                {
                    result.Add(new AthleteWithStatusDTO
                    {
                        User = FullUserModel.FromEntity(athlete),
                        Status = member?.Status.ToString() ?? "None"
                    });
                }
            }

            return result;
        }
    }
} 