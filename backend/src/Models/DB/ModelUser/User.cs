using System.ComponentModel.DataAnnotations;

namespace ShootingAcademy.Models.DB.ModelUser
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string SecoundName { get; set; }
        [Required]
        public string PatronymicName { get; set; }

        [Required]
        public int Age { get; set; }

        public string Grade { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public string RToken { get; set; }

        public List<GroupMember> AthleteGroups { get; set; } = [];
        public List<Course> InstructoredCourses { get; set; } = [];
        public List<CourseMember> Courses { get; set; } = [];
        public List<CompetitionMember> Competitions { get; set; } = [];
    }
}
