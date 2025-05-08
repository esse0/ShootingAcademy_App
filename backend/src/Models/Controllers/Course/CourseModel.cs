using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.Controllers.Course
{
    public class CourseModel
    {
        public string? Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string duration { get; set; }
        public string level { get; set; }
        public int rate { get; set; }
        public string category { get; set; }
        public int? peopleRateCount { get; set; }

        public bool? is_closed { get; set; }

        public FullUserModel? instructor { get; set; }

        public List<ModuleModel>? modules { get; set; }
        public List<FeatureModel>? features { get; set; }
        public List<FaqModel>? faqs { get; set; }
    }
}
