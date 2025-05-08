using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShootingAcademy.Models.DB
{
    public class Lesson
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ModuleId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public Module Module { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Oder { get; set; }

        public string VideoLink { get; set; }
    }
}
