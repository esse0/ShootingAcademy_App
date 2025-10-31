using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.Controllers.AthleteGroup
{
    public class AthleteWithStatusDTO
    {
        public FullUserModel User { get; set; }
        public string Status { get; set; }
    }
} 