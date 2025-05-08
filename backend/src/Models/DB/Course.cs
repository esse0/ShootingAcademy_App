using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.DB
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Duration { get; set; }

        public string Category { get; set; }

        public string Level { get; set; }

        public int Rate { get; set; }

        public int PeopleRateCount { get; set; }

        public bool IsClosed { get; set; }

        public Guid InstructorId { get; set; }
        [ForeignKey(nameof(InstructorId))]
        public User Instructor { get; set; }

        public List<CourseMember> Members { get; set; } = [];

        public List<Module> Modules { get; set; } = [];

        public List<Feature> Features { get; set; } = [];

        public List<Faq> Faqs { get; set; } = [];
    }
}
