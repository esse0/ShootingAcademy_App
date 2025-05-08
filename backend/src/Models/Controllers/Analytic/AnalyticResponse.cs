using ShootingAcademy.Models.DB;

namespace ShootingAcademy.Models.Controllers.Analytic
{
    public class AnalyticResponse
    {
        public int completedCoursesCount { get; set; }
        public int completedLessonsCount { get; set; }
        public int completedCompetitions {  get; set; }
        public List<CourseByCategory> coursesByCategory { get; set; }
        public StatisticHorizontalBar scoreByMonth { get; set; }
    }
}
