using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.TrainingSession;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingSessionController : BaseController
    {
        private readonly ITrainingSessionService _service;
        public TrainingSessionController(ITrainingSessionService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "coach")]
        public async Task<IActionResult> Create([FromBody] CreateTrainingSessionDTO dto)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _service.CreateAsync(dto, coachId);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "coach")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTrainingSessionDTO dto)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _service.UpdateAsync(id, dto, coachId);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "coach")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _service.DeleteAsync(id, coachId);
            return HandleResult();
        }

        [HttpPost("schedule")]
        [Authorize]
        public async Task<IActionResult> GetSchedule([FromBody] GetScheduleRequestDTO request)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _service.GetScheduleAsync(request.CoachId, request.AthleteId);
            return HandleResult(result);
        }

        [HttpGet("user-schedule")]
        [Authorize]
        public async Task<IActionResult> GetUserSchedule()
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _service.GetUserScheduleAsync(userId);
            return HandleResult(result);
        }

        [HttpGet("groups")]
        [Authorize(Roles = "coach")]
        public async Task<IActionResult> GetCoachGroups()
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _service.GetCoachGroupsAsync(coachId);
            return HandleResult(result);
        }

        [HttpGet("ranges")]
        [Authorize(Roles = "coach")]
        public async Task<IActionResult> GetAvailableRanges()
        {
            var coachId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _service.GetAvailableRangesAsync(coachId);
            return HandleResult(result);
        }
    }
} 