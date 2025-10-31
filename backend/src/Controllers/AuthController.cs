using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Auth;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SigninJwt([FromBody] SigninModel model)
        {
            var result = await _authService.SignInAsync(model, HttpContext);
            return HandleResult(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);
            return HandleResult(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var result = await _authService.RefreshAccessTokenAsync(HttpContext);
            return HandleResult(result);
        }

        [HttpPost("signout"), Authorize]
        public async Task<IActionResult> LogoutJwt()
        {
            var result = await _authService.LogoutAsync(HttpContext);
            return HandleResult(result);
        }
    }
}
