namespace ShootingAcademy.Models.Controllers.Course
{
    public class MyCourseBannerType
    {
        public string id { get; set; }

        public string title { get; set; }

        public string duration { get; set; }

        public string level { get; set; }

        public int completed_percent { get; set; }

        public bool is_closed { get; set; }

        public string category { get; set; }

        public string started_at { get; set; }
        public string finished_at { get; set; } = "";
    }
}
