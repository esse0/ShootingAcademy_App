using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Services;
using ShootingAcademy.Models.Controllers.Competition;
using System.Text.Json;

namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    public class CompetitionController : BaseController
    {
        private readonly ICompetitionService _competitionService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CompetitionController(ICompetitionService competitionService)
        {
            _competitionService = competitionService;
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                IncludeFields = true
            };
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var competitions = await _competitionService.GetAllCompetitionsAsync();
            return HandleResult(competitions);
        }

        [HttpGet("fulldata")]
        public async Task<IActionResult> GetCompetitionById([FromQuery] string competitionId)
        {
            var competition = await _competitionService.GetCompetitionByIdAsync(competitionId);
            return HandleResult(competition);
        }

        [HttpGet("user"), Authorize]
        public async Task<IActionResult> GetUserCompetitions([FromQuery] bool history = false)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var competitions = await _competitionService.GetUserCompetitionsAsync(userId, history);
            return HandleResult(competitions);
        }

        [HttpGet("organisator"), Authorize(Roles = "organization")]
        public async Task<IActionResult> GetOrganisatorCompetitions()
        {
            var organisatorId = AutorizeData.FromContext(HttpContext).UserGuid;
            var competitions = await _competitionService.GetOrganisatorCompetitionsAsync(organisatorId);
            return HandleResult(competitions);
        }

        [HttpPost("create"), Authorize(Roles = "organization")]
        public async Task<IActionResult> CreateCompetition([FromBody] CompetitionTypeResponse competition)
        {
            var organisatorId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _competitionService.CreateCompetitionAsync(competition, organisatorId);
            return HandleResult(result);
        }

        [HttpDelete("delete"), Authorize(Roles = "organization")]
        public async Task<IActionResult> DeleteCompetition([FromQuery] string competitionId)
        {
            var organisatorId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _competitionService.DeleteCompetitionAsync(competitionId, organisatorId);
            return HandleResult();
        }

        [HttpPost("addmember"), Authorize(Roles = "coach")]
        public async Task<IActionResult> AddMemberCompetition([FromQuery] string competitionId, [FromQuery] string userId)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _competitionService.AddMemberToCompetitionAsync(competitionId, userId, coachId);
            return HandleResult();
        }

        [HttpDelete("deletemember"), Authorize(Roles = "coach")]
        public async Task<IActionResult> DeleteMemberCompetition([FromQuery] string competitionId, [FromQuery] string userId)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _competitionService.DeleteMemberFromCompetitionAsync(competitionId, userId, coachId);
            return HandleResult();
        }

        [HttpGet("export"), Authorize]
        public async Task<IActionResult> ExportMembers([FromQuery] string competitionId)
        {
            var (content, fileName) = await _competitionService.ExportMembersAsync(competitionId);

            return File(content, "application/json", fileName);
        }

        [HttpPost("import"), Authorize(Roles = "organization")]
        public async Task<IActionResult> ImportMembers([FromQuery] string competitionId, [FromBody] CompetitionMemberResponse[] members)
        {
            var organisatorId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _competitionService.ImportMembersAsync(competitionId, members, organisatorId);
            return HandleResult();
        }

        [HttpPut("status"), Authorize(Roles = "organization")]
        public async Task<IActionResult> ChangeStatus([FromQuery] string competitionId, [FromQuery] string newStatus)
        {
            var organisatorId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _competitionService.ChangeCompetitionStatusAsync(competitionId, newStatus, organisatorId);
            return HandleResult();
        }

        [HttpGet("myathletesoutofcompetition"), Authorize(Roles = "coach")]
        public async Task<IActionResult> GetMyAthletesOutOfCompetition([FromQuery] string competitionId)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var athletes = await _competitionService.GetMyAthletesOutOfCompetitionAsync(competitionId, coachId);
            return HandleResult(athletes);
        }

        [HttpGet("myathletesinthecompetition"), Authorize(Roles = "coach")]
        public async Task<IActionResult> GetMyAthletesInTheCompetition([FromQuery] string competitionId)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var athletes = await _competitionService.GetMyAthletesInTheCompetitionAsync(competitionId, coachId);
            return HandleResult(athletes);
        }
    }
}
