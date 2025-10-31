using System.ComponentModel.DataAnnotations;
using ShootingAcademy.Models.DB;

namespace ShootingAcademy.Models.Controllers.Organization
{
    public class OrganizationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string CreatedAt { get; set; }
        public List<OrganizationMemberDto> Members { get; set; }
        public List<RangeDto> Ranges { get; set; }
    }

    public class OrganizationMemberDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string JoinedAt { get; set; }
    }

    public class RangeDto
    {
        public string Id { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateOrganizationDto
    {
        [Required(ErrorMessage = "Название организации обязательно")]
        [StringLength(100, ErrorMessage = "Название должно быть не более 100 символов")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Описание должно быть не более 500 символов")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен")]
        [Phone(ErrorMessage = "Неверный формат номера телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Адрес обязателен")]
        [StringLength(200, ErrorMessage = "Адрес должен быть не более 200 символов")]
        public string Address { get; set; }
    }

    public class UpdateOrganizationDto
    {
        [Required(ErrorMessage = "Название организации обязательно")]
        [StringLength(100, ErrorMessage = "Название должно быть не более 100 символов")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Описание должно быть не более 500 символов")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен")]
        [Phone(ErrorMessage = "Неверный формат номера телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Адрес обязателен")]
        [StringLength(200, ErrorMessage = "Адрес должен быть не более 200 символов")]
        public string Address { get; set; }
    }
} 