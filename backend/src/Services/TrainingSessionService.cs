using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Middleware;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.TrainingSession;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.Exceptions;

namespace ShootingAcademy.Services
{
    public interface ITrainingSessionService
    {
        Task<TrainingSessionDTO> CreateAsync(CreateTrainingSessionDTO dto, Guid coachId);
        Task<TrainingSessionDTO> UpdateAsync(Guid id, UpdateTrainingSessionDTO dto, Guid coachId);
        Task DeleteAsync(Guid id, Guid coachId);
        Task<IEnumerable<TrainingSessionDTO>> GetScheduleAsync(Guid? coachId, Guid? athleteId);
        Task<IEnumerable<TrainingSessionDTO>> GetUserScheduleAsync(Guid userId);
        Task<IEnumerable<GroupDTO>> GetCoachGroupsAsync(Guid coachId);
        Task<IEnumerable<RangeDTO>> GetAvailableRangesAsync(Guid coachId);
    }

    public class TrainingSessionService : ITrainingSessionService
    {
        private readonly ApplicationDbContext _context;
        public TrainingSessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TrainingSessionDTO> CreateAsync(CreateTrainingSessionDTO dto, Guid coachId)
        {
            if (!Enum.TryParse<TrainingSessionStatus>(dto.Status, true, out var status))
                throw new ValidationException(new Dictionary<string, string[]> { { "Status", new[] { "Invalid status value" } } });

            // Проверяем, что группа принадлежит тренеру
            var group = await _context.AthleteGroups
                .FirstOrDefaultAsync(g => g.Id == dto.GroupId && g.CoachId == coachId);
            if (group == null)
                throw new BaseException("Group not found or no access", 404);

            // Проверяем, что помещение доступно
            var range = await _context.Ranges
                .FirstOrDefaultAsync(r => r.Id == dto.RangeId && r.IsActive);
            if (range == null)
                throw new BaseException("Range not found or not active", 404);

            // Проверяем, нет ли пересечений по времени
            var hasOverlap = await _context.TrainingSessions
                .AnyAsync(s => s.RangeId == dto.RangeId && 
                    ((s.StartDate <= dto.StartDate && s.EndDate > dto.StartDate) ||
                     (s.StartDate < dto.EndDate && s.EndDate >= dto.EndDate)));
            if (hasOverlap)
                throw new ValidationException(new Dictionary<string, string[]> { { "Time", new[] { "Time slot overlaps with existing session" } } });

            var session = new TrainingSession
            {
                Id = Guid.NewGuid(),
                TrainerId = coachId,
                GroupId = dto.GroupId,
                RangeId = dto.RangeId,
                Status = status,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            _context.TrainingSessions.Add(session);
            await _context.SaveChangesAsync();
            return MapToDto(session);
        }

        public async Task<TrainingSessionDTO> UpdateAsync(Guid id, UpdateTrainingSessionDTO dto, Guid coachId)
        {
            var session = await _context.TrainingSessions
                .Include(s => s.Range)
                .FirstOrDefaultAsync(s => s.Id == id && s.TrainerId == coachId);
            if (session == null)
                throw new BaseException("Training session not found or no access", 404);

            if (!string.IsNullOrEmpty(dto.Status))
            {
                if (!Enum.TryParse<TrainingSessionStatus>(dto.Status, true, out var status))
                    throw new ValidationException(new Dictionary<string, string[]> { { "Status", new[] { "Invalid status value" } } });
                session.Status = status;
            }

            if (dto.StartDate.HasValue || dto.EndDate.HasValue)
            {
                var newStartDate = dto.StartDate ?? session.StartDate;
                var newEndDate = dto.EndDate ?? session.EndDate;

                // Проверяем пересечения только если изменилось время
                if (newStartDate != session.StartDate || newEndDate != session.EndDate)
                {
                    var hasOverlap = await _context.TrainingSessions
                        .AnyAsync(s => s.Id != id && s.RangeId == session.RangeId &&
                            ((s.StartDate <= newStartDate && s.EndDate > newStartDate) ||
                             (s.StartDate < newEndDate && s.EndDate >= newEndDate)));
                    if (hasOverlap)
                        throw new ValidationException(new Dictionary<string, string[]> { { "Time", new[] { "Time slot overlaps with existing session" } } });
                }

                session.StartDate = newStartDate;
                session.EndDate = newEndDate;
            }

            await _context.SaveChangesAsync();
            return MapToDto(session);
        }

        public async Task DeleteAsync(Guid id, Guid coachId)
        {
            var session = await _context.TrainingSessions.FirstOrDefaultAsync(s => s.Id == id && s.TrainerId == coachId);
            if (session == null)
                throw new BaseException("Training session not found or no access", 404);
            _context.TrainingSessions.Remove(session);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TrainingSessionDTO>> GetScheduleAsync(Guid? coachId, Guid? athleteId)
        {
            var query = _context.TrainingSessions
                .Include(s => s.Group)
                .Include(s => s.Trainer)
                .Include(s => s.Range)
                .AsQueryable();

            if (coachId.HasValue)
                query = query.Where(s => s.TrainerId == coachId.Value);
            if (athleteId.HasValue)
                query = query.Where(s => s.Group.Athletes.Any(a => a.AthleteId == athleteId.Value));

            var sessions = await query.ToListAsync();
            return sessions.Select(MapToDto);
        }

        public async Task<IEnumerable<TrainingSessionDTO>> GetUserScheduleAsync(Guid userId)
        {
            var sessions = await _context.TrainingSessions
                .Include(s => s.Group)
                .Include(s => s.Trainer)
                .Include(s => s.Range)
                .Where(s => s.Group.Athletes.Any(a => a.AthleteId == userId))
                .ToListAsync();

            return sessions.Select(MapToDto);
        }

        public async Task<IEnumerable<GroupDTO>> GetCoachGroupsAsync(Guid coachId)
        {
            var groups = await _context.AthleteGroups
                .Where(g => g.CoachId == coachId)
                .Select(g => new GroupDTO { Id = g.Id, Name = g.OrganisationName })
                .ToListAsync();
            return groups;
        }

        public async Task<IEnumerable<RangeDTO>> GetAvailableRangesAsync(Guid coachId)
        {
            // Получаем все активные помещения из организаций, где тренер является членом
            var ranges = await _context.Ranges
                .Include(r => r.Organization)
                .ThenInclude(o => o.Members)
                .Where(r => r.IsActive && r.Organization.Members.Any(m => m.UserId == coachId))
                .Select(r => new RangeDTO 
                { 
                    Id = r.Id, 
                    Name = r.Location, 
                    Location = r.Description 
                })
                .ToListAsync();
            return ranges;
        }

        private static TrainingSessionDTO MapToDto(TrainingSession s) => new TrainingSessionDTO
        {
            Id = s.Id,
            TrainerId = s.TrainerId,
            TrainerName = s.Trainer != null ? $"{s.Trainer.FirstName} {s.Trainer.SecoundName}" : null,
            GroupId = s.GroupId,
            GroupName = s.Group?.OrganisationName,
            RangeId = s.RangeId,
            RangeName = s.Range?.Location,
            RangeLocation = s.Range?.Description,
            Status = s.Status.ToString(),
            StartDate = s.StartDate,
            EndDate = s.EndDate
        };
    }
} 