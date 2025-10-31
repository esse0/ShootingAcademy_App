using System;

namespace ShootingAcademy.Models.Exceptions
{
    public class RangeNotFoundException : BaseException
    {
        public RangeNotFoundException(Guid rangeId) 
            : base($"Стрельбище с ID {rangeId} не найдено", 404)
        {
        }
    }

    public class RangeValidationException : BaseException
    {
        public RangeValidationException(string message) 
            : base(message, 400)
        {
        }
    }

    public class RangeAccessDeniedException : BaseException
    {
        public RangeAccessDeniedException(Guid rangeId, Guid userId) 
            : base($"Пользователь {userId} не имеет доступа к стрельбищу {rangeId}", 403)
        {
        }
    }
} 