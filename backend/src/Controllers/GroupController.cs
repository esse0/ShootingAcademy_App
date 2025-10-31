using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Group;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return HandleResult(groups);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserGroups()
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var groups = await _groupService.GetUserGroupsAsync(userId);
            return HandleResult(groups);
        }

        [HttpGet("coach"), Authorize(Roles = "coach")]
        public async Task<IActionResult> GetCoachGroups()
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var groups = await _groupService.GetCoachGroupsAsync(coachId);
            return HandleResult(groups);
        }

        [HttpGet("user/fulldata")]
        public async Task<IActionResult> GetUserGroup([FromQuery] string groupId)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var group = await _groupService.GetUserGroupAsync(groupId, userId);
            return HandleResult(group);
        }

        [HttpGet("coach/fulldata"), Authorize(Roles = "coach")]
        public async Task<IActionResult> GetCoachGroup([FromQuery] string groupId)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var group = await _groupService.GetCoachGroupAsync(groupId, coachId);
            return HandleResult(group);
        }

        [HttpPost("kickmember"), Authorize(Roles = "coach")]
        public async Task<IActionResult> KickMember([FromQuery] MemberData data)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _groupService.KickMemberAsync(data.groupId, data.userId, coachId);
            return HandleResult();
        }

        [HttpPost("create"), Authorize(Roles = "coach")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupModel group)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _groupService.CreateGroupAsync(group, coachId);
            return HandleResult();
        }

        [HttpDelete("delete"), Authorize(Roles = "coach")]
        public async Task<IActionResult> DeleteGroup([FromQuery] string groupId)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _groupService.DeleteGroupAsync(groupId, coachId);
            return HandleResult();
        }

        [HttpGet("unsubscribedathletes"), Authorize(Roles = "coach")]
        public async Task<IActionResult> GetUsersWithoutMembers([FromQuery] string groupId)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var athletes = await _groupService.GetUnsubscribedAthletesAsync(groupId, coachId);
            return HandleResult(athletes);
        }
    }
}