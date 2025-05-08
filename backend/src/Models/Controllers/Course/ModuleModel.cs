namespace ShootingAcademy.Models.Controllers.Course
{
    public class ModuleModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<LessonModel>? lessons { get; set; }
    }
}
