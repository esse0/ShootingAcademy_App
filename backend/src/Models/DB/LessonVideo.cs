using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShootingAcademy.Models.DB
{
    public class LessonVideo
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public int? DurationSec { get; set; }

        public Guid LessonId { get; set; }
        [ForeignKey(nameof(LessonId))]
        [InverseProperty("LessonVideo")]
        public Lesson Lesson { get; set; } = null!;
        
        public Guid MediaId { get; set; }
        [ForeignKey(nameof(MediaId))]
        public MediaStorage Media { get; set; } = null!;
    }
}
