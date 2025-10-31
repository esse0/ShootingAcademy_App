using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.User;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("auth"), Authorize]
        public IActionResult Auth()
        {
            return HandleResult();
        }

        [HttpPut, Authorize]
        public async Task<IActionResult> Put([FromBody] UserProfileData profileData)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var user = await _userService.UpdateUserProfileAsync(userId, profileData);
            return HandleResult(user);
        }

        [HttpDelete("deleteprofilephoto"), Authorize]
        public async Task<IActionResult> DeleteProfilePhoto()
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _userService.DeleteProfilePhotoAsync(userId);
            return HandleResult();
        }

        [HttpGet("get"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsersWithoutAdmin()
        {
            var users = await _userService.GetUsersWithoutAdminAsync();
            return HandleResult(users);
        }

        [HttpPost("changerole"), Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeUserRole([FromQuery] string userId, [FromQuery] string newRole)
        {
            await _userService.ChangeUserRoleAsync(userId, newRole);
            return HandleResult();
        }

        [HttpGet("organization"), Authorize(Roles = "organization, coach")]
        public async Task<IActionResult> GetUserOrganization()
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var userRole = AutorizeData.FromContext(HttpContext).Role;
            object organization = null;
            if (userRole == "organization")
            {
                organization = await _userService.GetUserOrganizationAsync(userId);
            }
            else if (userRole == "coach")
            {
                organization = await _userService.GetCoachOrganizationAsync(userId);
            }
            return HandleResult(organization);
        }
    }
}
