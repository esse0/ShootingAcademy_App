using ShootingAcademy.Models.DB.ModelUser;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShootingAcademy.Models.DB
{
    public class CourseMember
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public bool IsClosed { get; set; }

        [Column(TypeName = "jsonb")]
        public List<Guid> CompletedLessons { get; set; } = [];

        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}
