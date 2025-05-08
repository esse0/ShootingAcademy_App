namespace ShootingAcademy.Models.Controllers.Competition
{
    public class CompetitionMemberResponse
    {
        public string id { get; set; }
        public string fullName { get; set; }
        public int age { get; set; }
        public string country { get; set; }
        public string grade { get; set; }
        public float? result { get; set; }
    }
}
