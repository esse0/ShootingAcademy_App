using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.Controllers.AthleteGroup
{
    public class AthleteGroupModel
    {
        public string id { get; set; }
        public string organisationName { get; set; }

        public UserWithAvatar coach { get; set; }

        public List<FullUserModel> members { get; set; } = [];
    }
}
