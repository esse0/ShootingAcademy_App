namespace ShootingAcademy.Models.Controllers.Course
{
    public class ModuleWithVideoModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<LessonWithVideoModel>? lessons { get; set; }
    }
}
