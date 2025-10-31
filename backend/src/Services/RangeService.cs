using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Range;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.Exceptions;

namespace ShootingAcademy.Services
{
    public interface IRangeService
    {
        Task<RangeDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<RangeDTO>> GetByOrganizationIdAsync(Guid organizationId);
        Task<RangeDTO> CreateAsync(CreateRangeDTO createDto, Guid userId);
        Task<RangeDTO> UpdateAsync(Guid id, UpdateRangeDTO updateDto, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
    }

    public class RangeService : IRangeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrganizationService _organizationService;

        public RangeService(ApplicationDbContext context, IOrganizationService organizationService)
        {
            _context = context;
            _organizationService = organizationService;
        }

        public async Task<RangeDTO> GetByIdAsync(Guid id)
        {
            var range = await _context.Ranges
                .Include(r => r.Organization)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (range == null)
                throw new RangeNotFoundException(id);

            return MapToDto(range);
        }

        public async Task<IEnumerable<RangeDTO>> GetByOrganizationIdAsync(Guid organizationId)
        {
            var ranges = await _context.Ranges
                .Include(r => r.Organization)
                .Where(r => r.OrganizationId == organizationId)
                .ToListAsync();

            return ranges.Select(MapToDto);
        }

        public async Task<RangeDTO> CreateAsync(CreateRangeDTO createDto, Guid userId)
        {
            var organization = await _organizationService.GetByIdAsync(createDto.OrganizationId);
            
            var isOwner = organization.Members.Any(m => 
                m.UserId == userId.ToString() && 
                m.Status == Enum.GetName(typeof(OrganizationMemberStatus), OrganizationMemberStatus.Approved) && 
                m.Role == "owner");

            if (!isOwner)
                throw new RangeAccessDeniedException(Guid.Empty, userId);

            var range = new Models.DB.Range
            {
                Id = Guid.NewGuid(),
                OrganizationId = createDto.OrganizationId,
                Location = createDto.Location,
                Description = createDto.Description,
                Capacity = createDto.Capacity,
                Type = Enum.Parse<RangeType>(createDto.Type, true),
                IsActive = true
            };

            await _context.Ranges.AddAsync(range);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(range.Id);
        }

        public async Task<RangeDTO> UpdateAsync(Guid id, UpdateRangeDTO updateDto, Guid userId)
        {
            var range = await _context.Ranges
                .Include(r => r.Organization)
                .ThenInclude(o => o.Members)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (range == null)
                throw new RangeNotFoundException(id);

            // Проверяем, является ли пользователь владельцем организации
            var isOwner = range.Organization.Members.Any(m => 
                m.UserId == userId && 
                m.Status == OrganizationMemberStatus.Approved && 
                m.Role == "owner");

            if (!isOwner)
                throw new RangeAccessDeniedException(id, userId);

            if (updateDto.Location != null)
                range.Location = updateDto.Location;
            if (updateDto.Description != null)
                range.Description = updateDto.Description;
            if (updateDto.Capacity.HasValue)
                range.Capacity = updateDto.Capacity.Value;
            if (!string.IsNullOrEmpty(updateDto.Type))
                range.Type = Enum.Parse<RangeType>(updateDto.Type, true);
            if (updateDto.IsActive.HasValue)
                range.IsActive = updateDto.IsActive.Value;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var range = await _context.Ranges
                .Include(r => r.Organization)
                .ThenInclude(o => o.Members)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (range == null)
                throw new RangeNotFoundException(id);

            // Проверяем, является ли пользователь владельцем организации
            var isOwner = range.Organization.Members.Any(m => 
                m.UserId == userId && 
                m.Status == OrganizationMemberStatus.Approved && 
                m.Role == "owner");

            if (!isOwner)
                throw new RangeAccessDeniedException(id, userId);

            _context.Ranges.Remove(range);
            await _context.SaveChangesAsync();
        }

        private static RangeDTO MapToDto(Models.DB.Range range)
        {
            return new RangeDTO
            {
                Id = range.Id,
                OrganizationId = range.OrganizationId,
                Location = range.Location,
                Description = range.Description,
                Capacity = range.Capacity,
                Type = range.Type.ToString(),
                IsActive = range.IsActive
            };
        }
    }
} 