using System.ComponentModel.DataAnnotations;

namespace ShootingAcademy.Models.DB
{
    public class LessonVideo
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public int? DurationSec { get; set; }

        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
        public Guid MediaId { get; set; }
        public MediaStorage Media { get; set; } = null!;
    }

}
