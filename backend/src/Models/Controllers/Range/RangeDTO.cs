using System.ComponentModel.DataAnnotations;
using ShootingAcademy.Models.DB;

namespace ShootingAcademy.Models.Controllers.Range
{
    public class RangeDTO
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateRangeDTO
    {
        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        [Required]
        public string Type { get; set; }
    }

    public class UpdateRangeDTO
    {
        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 1000)]
        public int? Capacity { get; set; }

        public string Type { get; set; }

        public bool? IsActive { get; set; }
    }
} 