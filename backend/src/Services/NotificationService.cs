using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Notification;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using System.Text.RegularExpressions;

namespace ShootingAcademy.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDTO>> GetUserNotificationsAsync(Guid userId);
        Task<NotificationDTO> AcceptInvitationAsync(Guid notificationId, Guid userId);
        Task<NotificationDTO> DeclineInvitationAsync(Guid notificationId, Guid userId);
        Task<NotificationDTO> CreateNotificationAsync(CreateNotificationDTO dto);
        Task<NotificationDTO> GetNotificationAsync(Guid id, Guid userId);
        Task<NotificationDTO> UpdateNotificationAsync(Guid id, UpdateNotificationDTO dto, Guid userId);
        Task<NotificationDTO?> GetNotificationByInviteAndUser(Guid inviteId, Guid senderId, Guid invitedUserId);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationDTO> CreateNotificationAsync(CreateNotificationDTO dto)
        {
            ValidateCreateNotification(dto);

            var notification = new Models.DB.Notification
            {
                UserId = dto.UserId,
                SenderId = dto.SenderId,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                RequiresResponse = dto.RequiresResponse,
                IsRead = false,
                InviteId = dto.InviteId,
                CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return MapToDTO(notification);
        }

        public async Task<NotificationDTO> GetNotificationAsync(Guid id, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
            
            if (notification == null)
            {
                throw new NotificationNotFoundException(id);
            }

            if (notification.UserId != userId)
            {
                throw new NotificationAccessDeniedException(id, userId);
            }

            return MapToDTO(notification);
        }

        public async Task<List<NotificationDTO>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id.ToString(),
                    UserId = n.UserId.ToString(),
                    SenderId = n.SenderId.ToString(),
                    Type = n.Type,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    RequiresResponse = n.RequiresResponse,
                    Response = n.Response,
                    InviteId = n.InviteId.ToString(),
                    ResponseAt = n.ResponseAt.ToString(),
                    CreatedAt = n.CreatedAt.ToString()
                })
            .ToListAsync();
            return notifications;
        }

        public async Task<NotificationDTO?> GetNotificationByInviteAndUser(Guid inviteId, Guid senderId, Guid invitedUserId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.UserId == invitedUserId && n.Response == null && n.InviteId == inviteId && n.SenderId == senderId);

            if (notification == null)
                return null;

            return new NotificationDTO
            {
                Id = notification.Id.ToString(),
                UserId = notification.UserId.ToString(),
                SenderId = notification.SenderId.ToString(),
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                RequiresResponse = notification.RequiresResponse,
                Response = notification.Response,
                InviteId = notification.InviteId.ToString(),
                ResponseAt = notification.ResponseAt?.ToString(),
                CreatedAt = notification.CreatedAt.ToString()
            };
        }

        public async Task<NotificationDTO> UpdateNotificationAsync(Guid id, UpdateNotificationDTO dto, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
            
            if (notification == null)
            {
                throw new NotificationNotFoundException(id);
            }

            if (notification.UserId != userId)
            {
                throw new NotificationAccessDeniedException(id, userId);
            }

            ValidateUpdateNotification(dto);

            notification.IsRead = dto.IsRead;
            
            if (!string.IsNullOrEmpty(dto.Response))
            {
                if (!notification.RequiresResponse)
                {
                    throw new InvalidNotificationDataException("Это уведомление не требует ответа");
                }
                notification.Response = dto.Response;
                notification.ResponseAt = DateTime.UtcNow.ToUniversalTime();
            }

            await _context.SaveChangesAsync();
            return MapToDTO(notification);
        }

        public async Task DeleteNotificationAsync(Guid id, Guid userId)
        {
            var notification = await _context.Notifications.FindAsync(id);
            
            if (notification == null)
            {
                throw new NotificationNotFoundException(id);
            }

            if (notification.UserId != userId)
            {
                throw new NotificationAccessDeniedException(id, userId);
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<NotificationDTO> AcceptInvitationAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId)
                ?? throw new BaseException("Уведомление не найдено", 404);

            if (!notification.RequiresResponse)
            {
                throw new BaseException("Это уведомление не требует ответа", 400);
            }

            if (notification.Response != null)
            {
                throw new BaseException("На это уведомление уже дан ответ", 400);
            }

            notification.Response = "accepted";
            notification.ResponseAt = DateTime.UtcNow;
            notification.IsRead = true;

            await _context.SaveChangesAsync();

            return new NotificationDTO
            {
                Id = notification.Id.ToString(),
                UserId = notification.UserId.ToString(),
                SenderId = notification.SenderId.ToString(),
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                RequiresResponse = notification.RequiresResponse,
                Response = notification.Response,
                InviteId = notification.InviteId.ToString(),
                ResponseAt = notification.ResponseAt?.ToString(),
                CreatedAt = notification.CreatedAt.ToString()
            };
        }

        public async Task<NotificationDTO> DeclineInvitationAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId)
                ?? throw new BaseException("Уведомление не найдено", 404);

            if (!notification.RequiresResponse)
            {
                throw new BaseException("Это уведомление не требует ответа", 400);
            }

            if (notification.Response != null)
            {
                throw new BaseException("На это уведомление уже дан ответ", 400);
            }

            notification.Response = "declined";
            notification.ResponseAt = DateTime.UtcNow;
            notification.IsRead = true;

            await _context.SaveChangesAsync();

            return new NotificationDTO
            {
                Id = notification.Id.ToString(),
                UserId = notification.UserId.ToString(),
                SenderId = notification.SenderId.ToString(),
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                RequiresResponse = notification.RequiresResponse,
                Response = notification.Response,
                InviteId = notification.InviteId.ToString(),
                ResponseAt = notification.ResponseAt?.ToString(),
                CreatedAt = notification.CreatedAt.ToString()
            };
        }

        private static void ValidateCreateNotification(CreateNotificationDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new InvalidNotificationDataException("Заголовок уведомления не может быть пустым");
            }

            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                throw new InvalidNotificationDataException("Сообщение уведомления не может быть пустым");
            }

            if (dto.UserId == Guid.Empty)
            {
                throw new InvalidNotificationDataException("Неверный ID пользователя");
            }
        }

        private static void ValidateUpdateNotification(UpdateNotificationDTO dto)
        {
            if (dto.Response != null && dto.Response.Length > 500)
            {
                throw new InvalidNotificationDataException("Длина ответа не может превышать 500 символов");
            }
        }

        private NotificationDTO MapToDTO(Models.DB.Notification notification)
        {
            return new NotificationDTO
            {
                Id = notification.Id.ToString(),
                UserId = notification.UserId.ToString(),
                SenderId = notification.SenderId.ToString(),
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                InviteId = notification.InviteId.ToString(),
                RequiresResponse = notification.RequiresResponse,
                Response = notification.Response,
                ResponseAt = notification.ResponseAt?.ToUniversalTime().ToString(),
                CreatedAt = notification.CreatedAt.ToUniversalTime().ToString()
            };
        }
    }
} 