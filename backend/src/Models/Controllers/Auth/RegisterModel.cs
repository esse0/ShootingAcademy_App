using System.ComponentModel.DataAnnotations;

namespace ShootingAcademy.Models.Controllers.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Укажите ваше имя")]
        public string name {  get; set; }

        [Required(ErrorMessage = "Укажите ваше имя")]
        public string lastName { get; set; }

        [EmailAddress(ErrorMessage = "Почта не верная")]
        [Required(ErrorMessage = "Укажите вашу почту")]
        public string email {  get; set; }

        [Required(ErrorMessage = "Укажите ваш пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть не короче 6-ти символов")]
        [DataType(DataType.Password, ErrorMessage = "Пароль не верный")]
        public string password { get; set; }
    }
}
