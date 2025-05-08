using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.Controllers.Competition
{
    public class CompetitionTypeResponse
    {
        public string? id {  get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public int? memberCount { get; set; }
        public int maxMemberCount { get; set; }
        public string venue {  get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string? status { get; set; }
        public string exercise { get; set; }
        public string? organiser { get; set; }
        public List<CompetitionMemberResponse>? members { get; set; }
    }
}
