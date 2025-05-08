using ShootingAcademy.Models.DB;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShootingAcademy.Models.Controllers.Group
{
    public class GroupModel
    {
        public Guid? Id { get; set; }

        public Guid? CoachId { get; set; }

        public string OrganisationName { get; set; }

        public List<GroupMember>? Athletes { get; set; } = [];
    }
}
