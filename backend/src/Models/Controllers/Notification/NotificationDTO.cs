using ShootingAcademy.Models.DB;
using System;

namespace ShootingAcademy.Models.Controllers.Notification
{
    public class NotificationDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string? SenderId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public bool RequiresResponse { get; set; }
        public string? Response { get; set; }
        public string? InviteId { get; set; }
        public string? ResponseAt { get; set; }
        public string CreatedAt { get; set; }
    }

    public class CreateNotificationDTO
    {
        public Guid UserId { get; set; }
        public Guid? SenderId { get; set; }
        public NotificationType Type { get; set; }
        public Guid? InviteId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool RequiresResponse { get; set; }
    }

    public class UpdateNotificationDTO
    {
        public bool IsRead { get; set; }
        public string Response { get; set; }
    }
}
