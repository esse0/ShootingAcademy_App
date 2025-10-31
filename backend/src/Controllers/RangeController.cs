using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Range;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RangeController : ControllerBase
    {
        private readonly IRangeService _rangeService;

        public RangeController(IRangeService rangeService)
        {
            _rangeService = rangeService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RangeDTO>> GetById(Guid id)
        {
            var range = await _rangeService.GetByIdAsync(id);
            return Ok(range);
        }

        [HttpGet("organization/{organizationId}")]
        [Authorize(Roles = "organization,coach")]
        public async Task<ActionResult<IEnumerable<RangeDTO>>> GetByOrganizationId(Guid organizationId)
        {
          
            var ranges = await _rangeService.GetByOrganizationIdAsync(organizationId);
            return Ok(ranges);
        }

        [HttpPost]
        [Authorize(Roles = "organization")]
        public async Task<ActionResult<RangeDTO>> Create([FromBody] CreateRangeDTO createDto)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var range = await _rangeService.CreateAsync(createDto, userId);

            return CreatedAtAction(nameof(GetById), new { id = range.Id }, range);

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "organization")]
        public async Task<ActionResult<RangeDTO>> Update(Guid id, [FromBody] UpdateRangeDTO updateDto)
        {

            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var range = await _rangeService.UpdateAsync(id, updateDto, userId);
            return Ok(range);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "organization")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _rangeService.DeleteAsync(id, userId);
            return NoContent();

        }
    }
} 