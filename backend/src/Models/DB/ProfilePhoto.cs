using ShootingAcademy.Models.DB.ModelUser;
using System.ComponentModel.DataAnnotations;

namespace ShootingAcademy.Models.DB
{
    public class ProfilePhoto
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MediaId {  get; set; }
        public MediaStorage Media { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }

}
