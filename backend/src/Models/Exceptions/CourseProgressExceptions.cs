namespace ShootingAcademy.Models.Exceptions
{
    public class CourseProgressNotFoundException : BaseException
    {
        public CourseProgressNotFoundException(string courseId, Guid userId) 
            : base($"Пользователь {userId} не зарегистрирован на курс {courseId}", 404)
        {
        }
    }

    public class CourseProgressValidationException : BaseException
    {
        public CourseProgressValidationException(string message) 
            : base(message, 400)
        {
        }
    }

    public class CourseProgressAccessDeniedException : BaseException
    {
        public CourseProgressAccessDeniedException(string courseId, Guid userId) 
            : base($"Пользователь {userId} не имеет доступа к курсу {courseId}", 403)
        {
        }
    }

    public class LessonNotFoundException : BaseException
    {
        public LessonNotFoundException(string lessonId, string courseId) 
            : base($"Урок {lessonId} не найден в курсе {courseId}", 404)
        {
        }
    }
} 