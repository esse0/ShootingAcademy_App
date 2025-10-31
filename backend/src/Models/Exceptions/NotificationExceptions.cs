using System;

namespace ShootingAcademy.Models.Exceptions
{
    public class NotificationNotFoundException : BaseException
    {
        public NotificationNotFoundException(Guid notificationId) 
            : base($"Уведомление с ID {notificationId} не найдено", 404)
        {
        }
    }

    public class NotificationAccessDeniedException : BaseException
    {
        public NotificationAccessDeniedException(Guid notificationId, Guid userId) 
            : base($"Пользователь {userId} не имеет доступа к уведомлению {notificationId}", 403)
        {
        }
    }

    public class InvalidNotificationDataException : BaseException
    {
        public InvalidNotificationDataException(string message) 
            : base(message, 400)
        {
        }
    }
} 