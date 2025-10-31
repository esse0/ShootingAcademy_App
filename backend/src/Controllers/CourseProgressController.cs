using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Course;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CourseProgressController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseProgressController> _logger;

        public CourseProgressController(ICourseService courseService, ILogger<CourseProgressController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet("{courseId}")]
        public async Task<IResult> GetProgress(string courseId)
        {
            try
            {
                var userId = AutorizeData.FromContext(HttpContext).UserGuid;
                var progress = await _courseService.GetCourseProgressAsync(courseId, userId);
                return Results.Ok(progress);
            }
            catch (BaseException ex)
            {
                _logger.LogError(ex, "Error getting course progress");
                return Results.Json(ex.GetModel(), statusCode: ex.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting course progress");
                throw new BaseException("Произошла ошибка при получении прогресса курса", 500);
            }
        }

        [HttpPost("{courseId}/lessons/{lessonId}/complete")]
        public async Task<IResult> MarkLessonAsCompleted(string courseId, string lessonId)
        {
            try
            {
                var userId = AutorizeData.FromContext(HttpContext).UserGuid;
                await _courseService.MarkLessonAsCompletedAsync(courseId, lessonId, userId);
                return Results.Ok(new { message = "Урок отмечен как пройденный" });
            }
            catch (BaseException ex)
            {
                _logger.LogError(ex, "Error marking lesson as completed");
                return Results.Json(ex.GetModel(), statusCode: ex.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error marking lesson as completed");
                throw new BaseException("Произошла ошибка при отметке урока как пройденного", 500);
            }
        }

        [HttpDelete("{courseId}/lessons/{lessonId}/complete")]
        public async Task<IResult> UnmarkLessonAsCompleted(string courseId, string lessonId)
        {
            try
            {
                var userId = AutorizeData.FromContext(HttpContext).UserGuid;
                await _courseService.UnmarkLessonAsCompletedAsync(courseId, lessonId, userId);
                return Results.Ok(new { message = "Отметка о прохождении урока отменена" });
            }
            catch (BaseException ex)
            {
                _logger.LogError(ex, "Error unmarking lesson as completed");
                return Results.Json(ex.GetModel(), statusCode: ex.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error unmarking lesson as completed");
                throw new BaseException("Произошла ошибка при отмене отметки о прохождении урока", 500);
            }
        }
    }
} 