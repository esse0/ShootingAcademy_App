using System.ComponentModel.DataAnnotations;

namespace ShootingAcademy.Models.DB
{
    public class Organization
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description {  get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToUniversalTime();

        public List<OrganizationMembership> Members { get; set; }
        public List<Range> Ranges { get; set; }
    }
}
