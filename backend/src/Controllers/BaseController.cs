using Microsoft.AspNetCore.Mvc;

namespace ShootingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(T result)
        {
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        protected IActionResult HandleResult(object result)
        {
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        protected IActionResult HandleResult()
        {
            return Ok(new { message = "Operation completed successfully" });
        }
    }
} 