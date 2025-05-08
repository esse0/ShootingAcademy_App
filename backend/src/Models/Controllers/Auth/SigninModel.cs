using System.ComponentModel.DataAnnotations;

namespace ShootingAcademy.Models.Controllers.Auth
{
    public class SigninModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
