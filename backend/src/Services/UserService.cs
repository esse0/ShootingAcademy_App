using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Organization;
using ShootingAcademy.Models.Controllers.TrainingSession;
using ShootingAcademy.Models.Controllers.User;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services.Media;

namespace ShootingAcademy.Services
{
    public interface IUserService
    {
        Task<UserWithAvatar> UpdateUserProfileAsync(Guid userId, UserProfileData profileData);
        Task DeleteProfilePhotoAsync(Guid userId);
        Task<IEnumerable<FullUserModel>> GetUsersWithoutAdminAsync();
        Task ChangeUserRoleAsync(string userId, string newRole);
        Task<OrganizationDto> GetUserOrganizationAsync(Guid userId);
        Task<OrganizationDto> GetCoachOrganizationAsync(Guid userId);
        Task<IEnumerable<TrainingSessionDTO>> GetScheduleAsync(Guid? coachId, Guid? athleteId);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly ITrainingSessionService _trainingSessionService;

        public UserService(ApplicationDbContext context, IImageService imageService, ITrainingSessionService trainingSessionService)
        {
            _context = context;
            _imageService = imageService;
            _trainingSessionService = trainingSessionService;
        }

        public async Task<UserWithAvatar> UpdateUserProfileAsync(Guid userId, UserProfileData profileData)
        {
            if (await _context.Users.AnyAsync(usr => usr.Id != userId && usr.Email == profileData.Email))
            {
                throw new BaseException("Данная почта занята!");
            }

            var user = await _context.Users.FirstAsync(usr => usr.Id == userId);

            user.FirstName = profileData.Name;
            user.SecoundName = profileData.LastName;
            user.Address = profileData.Address;
            user.City = profileData.City;
            user.Country = profileData.Country;
            user.Grade = profileData.Grade;
            user.Age = profileData.Age;
            user.Email = profileData.Email;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var profileImage = await _imageService.GetFileUrl(user.Id);
            return UserWithAvatar.FromEntity(user, profileImage.FileUri);
        }

        public async Task DeleteProfilePhotoAsync(Guid userId)
        {
            await _imageService.DeleteProfilePhoto(userId);
        }

        public async Task<IEnumerable<FullUserModel>> GetUsersWithoutAdminAsync()
        {
            var users = await _context.Users
                .Where(u => u.Role != "admin")
                .AsNoTracking()
                .ToListAsync();

            return users.Select(FullUserModel.FromEntity);
        }

        public async Task ChangeUserRoleAsync(string userId, string newRole)
        {
            string[] allowedRoles = ["athlete", "coach", "organization"];

            if (!allowedRoles.Contains(newRole))
            {
                throw new BaseException("Role not supported", 400);
            }

            var user = await _context.Users.FindAsync(Guid.Parse(userId)) 
                ?? throw new BaseException("User not found", 404);

            if (user.Role == "admin" && newRole != "admin")
            {
                throw new BaseException("Cannot change administrator role", 400);
            }

            var existingMembership = await _context.OrganizationMemberships
                   .FirstOrDefaultAsync(m => m.UserId == user.Id && m.Role == "owner" && m.Status == OrganizationMemberStatus.Approved);

            if (newRole == "organization" && existingMembership == null)
            {
                // Создаем новую организацию
                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = $"{user.FirstName} {user.SecoundName} Organization",
                    Description = "Автоматически созданная организация",
                    Email = user.Email,
                    PhoneNumber = "",
                    Address = user.Address ?? "",
                    CreatedAt = DateTime.UtcNow.ToUniversalTime()
                };

                await _context.Organizations.AddAsync(organization);

                // Добавляем пользователя как владельца организации
                var membership = new OrganizationMembership
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organization.Id,
                    UserId = user.Id,
                    Role = "owner",
                    Status = OrganizationMemberStatus.Approved,
                    JoinedAt = DateTime.UtcNow.ToUniversalTime()
                };

                await _context.OrganizationMemberships.AddAsync(membership);
            }

            user.Role = newRole;
            await _context.SaveChangesAsync();
        }

        public async Task<OrganizationDto> GetUserOrganizationAsync(Guid userId)
        {
            try
            {
                var organizationMembership = await _context.OrganizationMemberships
                    .Include(m => m.Organization)
                        .ThenInclude(o => o.Members)
                            .ThenInclude(m => m.User)
                    .Include(m => m.Organization)
                        .ThenInclude(o => o.Ranges)
                    .FirstOrDefaultAsync(m => m.UserId == userId && m.Role == "owner" && m.Status == OrganizationMemberStatus.Approved)
                    ?? throw new BaseException("Организация не найдена", 404);

                return new OrganizationDto
                {
                    Id = organizationMembership.Organization.Id.ToString(),
                    Name = organizationMembership.Organization.Name,
                    Description = organizationMembership.Organization.Description,
                    Email = organizationMembership.Organization.Email,
                    PhoneNumber = organizationMembership.Organization.PhoneNumber,
                    Address = organizationMembership.Organization.Address,
                    CreatedAt = organizationMembership.Organization.CreatedAt.ToString(),
                    Members = organizationMembership.Organization.Members?.Select(m => new OrganizationMemberDto
                    {
                        UserId = m.UserId.ToString(),
                        UserName = m.User?.FirstName + " " + m.User?.SecoundName + " " + m.User?.PatronymicName,
                        Status = Enum.GetName(typeof(OrganizationMemberStatus), m.Status),
                        Role = m.Role,
                        JoinedAt = m.JoinedAt.ToUniversalTime().ToString(),
                    }).ToList(),
                    Ranges = organizationMembership.Organization.Ranges?.Select(r => new RangeDto
                    {
                        Id = r.Id.ToString(),
                        Location = r.Location,
                        Description = r.Description,
                        Capacity = r.Capacity,
                        Type = Enum.GetName(typeof(RangeType), r.Type),
                        IsActive = r.IsActive
                    }).ToList()
                };
            }
            catch (Exception ex) when (ex is not BaseException)
            {
                throw new BaseException("Произошла ошибка при получении организации", 500);
            }
        }

        public async Task<OrganizationDto> GetCoachOrganizationAsync(Guid userId)
        {
            try
            {
                var organizationMembership = await _context.OrganizationMemberships
                    .Include(m => m.Organization)
                        .ThenInclude(o => o.Members)
                            .ThenInclude(m => m.User)
                    .Include(m => m.Organization)
                        .ThenInclude(o => o.Ranges)
                    .FirstOrDefaultAsync(m => m.UserId == userId && m.Role == "coach" && m.Status == OrganizationMemberStatus.Approved)
                    ?? throw new BaseException("Организация не найдена", 404);

                return new OrganizationDto
                {
                    Id = organizationMembership.Organization.Id.ToString(),
                    Name = organizationMembership.Organization.Name,
                    Description = organizationMembership.Organization.Description,
                    Email = organizationMembership.Organization.Email,
                    PhoneNumber = organizationMembership.Organization.PhoneNumber,
                    Address = organizationMembership.Organization.Address,
                    CreatedAt = organizationMembership.Organization.CreatedAt.ToString(),
                    Members = organizationMembership.Organization.Members?.Select(m => new OrganizationMemberDto
                    {
                        UserId = m.UserId.ToString(),
                        UserName = m.User?.FirstName + " " + m.User?.SecoundName + " " + m.User?.PatronymicName,
                        Status = Enum.GetName(typeof(OrganizationMemberStatus), m.Status),
                        Role = m.Role,
                        JoinedAt = m.JoinedAt.ToUniversalTime().ToString(),
                    }).ToList(),
                    Ranges = organizationMembership.Organization.Ranges?.Select(r => new RangeDto
                    {
                        Id = r.Id.ToString(),
                        Location = r.Location,
                        Description = r.Description,
                        Capacity = r.Capacity,
                        Type = Enum.GetName(typeof(RangeType), r.Type),
                        IsActive = r.IsActive
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new BaseException($"Ошибка при получении организации тренера: {ex.Message}", 500);
            }
        }

        public async Task<IEnumerable<TrainingSessionDTO>> GetScheduleAsync(Guid? coachId, Guid? athleteId)
        {
            return await _trainingSessionService.GetScheduleAsync(coachId, athleteId);
        }
    }
} 