namespace ShootingAcademy.Models.Controllers.Course
{
    public class CourseProgressModel
    {
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public double ProgressPercentage { get; set; }
        public List<string> CompletedLessonIds { get; set; } = [];
    }
} 