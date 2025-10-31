using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Course;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    public class CourseController : BaseController
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return HandleResult(courses);
        }

        [HttpGet("user"), Authorize]
        public async Task<IActionResult> GetUserCourses([FromQuery] bool history = false)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var courses = await _courseService.GetUserCoursesAsync(userId, history);
            return HandleResult(courses);
        }

        [HttpGet("fulldata")]
        public async Task<IActionResult> GetCourseFullData([FromQuery] string id)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var courseData = await _courseService.GetCourseFullDataAsync(id,  userId);
            return HandleResult(courseData);
        }

        [HttpPost("subscribe"), Authorize]
        public async Task<IActionResult> SubscribeUser([FromQuery] string courseId)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _courseService.SubscribeUserAsync(courseId, userId);
            return HandleResult();
        }

        [HttpPut("leaveCourse"), Authorize]
        public async Task<IActionResult> UnsubscribeUser([FromQuery] string courseId)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _courseService.UnsubscribeUserAsync(courseId, userId);
            return HandleResult();
        }

        [HttpGet("admin"), Authorize(Roles = "organization")]
        public async Task<IActionResult> GetAdminCourses()
        {
            var instructorId = AutorizeData.FromContext(HttpContext).UserGuid;
            var courses = await _courseService.GetAdminCoursesAsync(instructorId);
            return HandleResult(courses);
        }

        [HttpPost("create"), Authorize(Roles = "organization")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseModel course)
        {
            Guid instructorId = AutorizeData.FromContext(HttpContext).UserGuid;

            var courseRes = await _courseService.CreateCourseAsync(course, instructorId);
            return HandleResult();
             
        }

        [HttpDelete("delete"), Authorize(Roles = "organization")]
        public async Task<IActionResult> DeleteCourse([FromQuery] string courseId)
        {
            var instructorId = AutorizeData.FromContext(HttpContext).UserGuid;
            await _courseService.DeleteCourseAsync(courseId, instructorId);
            return HandleResult();
        }
    }
}
