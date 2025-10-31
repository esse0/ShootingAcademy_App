using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Organization;
using ShootingAcademy.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ValidationException = ShootingAcademy.Middleware.ValidationException;

namespace ShootingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "organization")]
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationService _organizationService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public OrganizationController(
            IOrganizationService organizationService,
            ProblemDetailsFactory problemDetailsFactory)
        {
            _organizationService = organizationService;
            _problemDetailsFactory = problemDetailsFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var organizations = await _organizationService.GetAllAsync();
            return HandleResult(organizations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var organization = await _organizationService.GetByIdAsync(id);
            return HandleResult(organization);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganizationDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var problemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                    HttpContext,
                    ModelState);
                throw ValidationException.FromProblemDetails(problemDetails);
            }

            var createdOrganization = await _organizationService.CreateAsync(createDto);
            return HandleResult(createdOrganization);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateOrganizationDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var problemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                    HttpContext,
                    ModelState);
                throw ValidationException.FromProblemDetails(problemDetails);
            }

            var updatedOrganization = await _organizationService.UpdateAsync(id, updateDto);
            return HandleResult(updatedOrganization);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _organizationService.DeleteAsync(id);
            return HandleResult();
        }

        [HttpGet("uninvitedusers")]
        [Authorize]
        public async Task<IActionResult> GetUninvitedUsers()
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var users = await _organizationService.GetUninvitedUsersAsync(userId);
            return HandleResult(users);
        }

        [HttpPut("member/reject")]
        public async Task<IActionResult> RejectMember([FromQuery] Guid organizationId, [FromQuery] Guid userId)
        {
            var ownerId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _organizationService.RejectCoachAsync(organizationId, userId, ownerId);
            return Ok();
        }
    }
}
