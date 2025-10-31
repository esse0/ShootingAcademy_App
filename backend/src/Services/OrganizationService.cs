using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.Controllers.Organization;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Models;
using ShootingAcademy.Services.Media;
using ShootingAcademy.Models.DB.ModelUser;
using static ShootingAcademy.Services.OrganizationService;

namespace ShootingAcademy.Services
{
    public interface IOrganizationService
    {
        Task<OrganizationDto> GetByIdAsync(Guid id);
        Task<IEnumerable<OrganizationDto>> GetAllAsync();
        Task<OrganizationDto> CreateAsync(CreateOrganizationDto createDto);
        Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationDto updateDto);
        Task DeleteAsync(Guid id);
        Task InviteMemberAsync(Guid organizationId, Guid invitedUserId, Guid senderId);
        Task UpdateStatusMemberAsync(Guid organizationId, Guid userId, Guid senderId, OrganizationMemberStatus status);
        Task<List<UninvitedUserDto>> GetUninvitedUsersAsync(Guid ownerId);
        Task<bool> CancelOrganizationInvitationAsync(Guid organizationId, Guid invitedUserId, Guid ownerId);
        Task<bool> RejectCoachAsync(Guid organizationId, Guid coachUserId, Guid ownerId);
    }

    public class OrganizationService : IOrganizationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public OrganizationService(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<OrganizationDto> GetByIdAsync(Guid id)
        {
            var organization = await _context.Organizations
                .Include(o => o.Members)
                    .ThenInclude(m => m.User)
                .Include(o => o.Ranges)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (organization == null)
                throw new OrganizationNotFoundException(id);

            return MapToDto(organization);
        }

        public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
        {
            var organizations = await _context.Organizations
                .Include(o => o.Members)
                    .ThenInclude(m => m.User)
                .Include(o => o.Ranges)
                .ToListAsync();
            return organizations.Select(MapToDto);
        }

        public async Task<OrganizationDto> CreateAsync(CreateOrganizationDto createDto)
        {
            if (await _context.Organizations.AnyAsync(o => o.Email == createDto.Email))
                throw new OrganizationValidationException("Организация с таким email уже существует");

            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Description = createDto.Description,
                Email = createDto.Email,
                PhoneNumber = createDto.PhoneNumber,
                Address = createDto.Address
            };

            await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(organization.Id);
        }

        public async Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationDto updateDto)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null)
                throw new OrganizationNotFoundException(id);

            if (updateDto.Email != null && updateDto.Email != organization.Email)
            {
                if (await _context.Organizations.AnyAsync(o => o.Email == updateDto.Email))
                    throw new OrganizationValidationException("Организация с таким email уже существует");
                organization.Email = updateDto.Email;
            }

            if (updateDto.Name != null)
                organization.Name = updateDto.Name;
            if (updateDto.Description != null)
                organization.Description = updateDto.Description;
            if (updateDto.PhoneNumber != null)
                organization.PhoneNumber = updateDto.PhoneNumber;
            if (updateDto.Address != null)
                organization.Address = updateDto.Address;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null)
                throw new OrganizationNotFoundException(id);

            _context.Organizations.Remove(organization);
            await _context.SaveChangesAsync();
        }

        public async Task InviteMemberAsync(Guid organizationId, Guid invitedUserId, Guid senderId)
        {
            var athlete = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == invitedUserId && u.Role == "coach");

            if (athlete == null)
            {
                throw new BaseException("Coach not found.", code: 404);
            }

            var organization = await _context.Organizations
                .Include(o => o.Members)
                .FirstOrDefaultAsync(o => o.Id == organizationId);

            if (organization == null)
                throw new OrganizationNotFoundException(organizationId);

            var senderMembership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == senderId);

            if (senderMembership == null || senderMembership.Role != "owner")
            {
                throw new OrganizationValidationException("Вы не являетесь создателем");
            }

            if (organization.Members.Any(m => m.UserId == invitedUserId))
            {
                if(organization.Members.Any(m => m.UserId == invitedUserId && m.Status == OrganizationMemberStatus.Pending))
                {
                    throw new OrganizationValidationException("User alredy invited");
                }

                await UpdateStatusMemberAsync(organizationId, invitedUserId, senderId, OrganizationMemberStatus.Pending);
                return;
            }
                

            var membership = new OrganizationMembership
            {
                OrganizationId = organizationId,
                UserId = invitedUserId,
                Status = OrganizationMemberStatus.Pending,
                Role = "coach",
                JoinedAt = DateTime.UtcNow.ToUniversalTime()
            };

            await _context.OrganizationMemberships.AddAsync(membership);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusMemberAsync(Guid organizationId, Guid userId, Guid senderId, OrganizationMemberStatus status)
        {
            var membership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == userId);

            if (membership == null)
                throw new OrganizationValidationException("Участник не найден");

            var senderMembership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == senderId);

            if (senderMembership == null || senderMembership.Role != "owner")
            {
                throw new OrganizationValidationException("Вы не являетесь создателем");
            }

            membership.Status = status;

            await _context.SaveChangesAsync();
        }

        public class UninvitedUserDto
        {
            public FullUserModel User { get; set; }
            public string Status { get; set; } // "Pending", "Rejected", null
        }

        public async Task<List<UninvitedUserDto>> GetUninvitedUsersAsync(Guid ownerId)
        {
            var ownerMembership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(m => m.UserId == ownerId && m.Role == "owner" && m.Status == OrganizationMemberStatus.Approved);

            if (ownerMembership == null)
                throw new BaseException("Организация не найдена", 404);

            var organizationId = ownerMembership.OrganizationId;

            // Получаем все membership коучей этой организации
            var allCoachMemberships = await _context.OrganizationMemberships
                .Where(m => m.OrganizationId == organizationId && m.Role== "coach")
                .ToListAsync();

            // Получаем всех коучей
            var allCoaches = await _context.Users
                .Where(u => u.Role == "coach")
                .ToListAsync();

            var result = new List<UninvitedUserDto>();
            foreach (var user in allCoaches)
            {
                var membership = allCoachMemberships.FirstOrDefault(m => m.UserId == user.Id);
                // Добавляем только если membership нет или статус не Approved
                if (membership == null || membership.Status != OrganizationMemberStatus.Approved)
                {
                    string? status = null;
                    if (membership != null)
                        status = Enum.GetName(typeof(OrganizationMemberStatus), membership.Status);
                    result.Add(new UninvitedUserDto
                    {
                        User = FullUserModel.FromEntity(user),
                        Status = status
                    });
                }
            }
            return result;
        }

        public async Task<bool> CancelOrganizationInvitationAsync(Guid organizationId, Guid invitedUserId, Guid ownerId)
        {
            var ownerMembership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == ownerId && m.Role == "owner" && m.Status == OrganizationMemberStatus.Approved);

            if (ownerMembership == null)
                throw new BaseException("Вы не являетесь владельцем организации", 403);

            var membership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == invitedUserId && m.Status == OrganizationMemberStatus.Pending);

            if (membership == null)
                throw new BaseException("Приглашение не найдено", 404);

            _context.OrganizationMemberships.Remove(membership);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectCoachAsync(Guid organizationId, Guid coachUserId, Guid ownerId)
        {
            var ownerMembership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == ownerId && m.Role == "owner" && m.Status == OrganizationMemberStatus.Approved);

            if (ownerMembership == null)
                throw new BaseException("Вы не являетесь владельцем организации", 403);

            var membership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == coachUserId && m.Role == "coach" && m.Status == OrganizationMemberStatus.Approved);

            if (membership == null)
                throw new BaseException("Можно отклонить только coach со статусом Approved", 404);

            membership.Status = OrganizationMemberStatus.Rejected;
            await _context.SaveChangesAsync();
            return true;
        }

        private static OrganizationDto MapToDto(Organization organization)
        {
            return new OrganizationDto
            {
                Id = organization.Id.ToString(),
                Name = organization.Name,
                Description = organization.Description,
                Email = organization.Email,
                PhoneNumber = organization.PhoneNumber,
                Address = organization.Address,
                CreatedAt = organization.CreatedAt.ToString(),
                Members = organization.Members?.Select(m => new OrganizationMemberDto
                {
                    UserId = m.UserId.ToString(),
                    UserName = m.User?.FirstName + " " + m.User?.SecoundName + " " + m.User?.PatronymicName,
                    Status = Enum.GetName(typeof(OrganizationMemberStatus), m.Status),
                    Role = m.Role,
                    JoinedAt = m.JoinedAt.ToUniversalTime().ToString(),
                }).ToList(),
                Ranges = organization.Ranges?.Select(r => new RangeDto
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
    }
} 