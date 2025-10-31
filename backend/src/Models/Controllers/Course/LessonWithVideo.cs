namespace ShootingAcademy.Models.Controllers.Course
{
    public class LessonWithVideoModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string? videoId { get; set; }
        public string? videoUri { get; set; }
        public bool isCompleted { get; set; }
    }
}
