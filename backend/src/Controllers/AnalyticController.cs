using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Models;
using ShootingAcademy.Services;
using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models.Controllers.Analytic;
using static ShootingAcademy.Models.DB.Competition;

namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public AnalyticController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet, Authorize]
        public async Task<IResult> GetAnalitics()
        {
            try
            {
                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                User user = await dbContext.Users
                .Include(usr => usr.Courses)
                    .ThenInclude(course => course.Course)
                    .ThenInclude(course => course.Modules)
                    .ThenInclude(module => module.Lessons)
                .Include(usr => usr.Competitions)
                    .ThenInclude(competition => competition.Competition)
                .FirstAsync(usr => usr.Id == userGuid);

                var completedCoursesCount = user.Courses.Count(x => x.IsClosed == true);
                var completedLessonsCount = user.Courses
                    .Where(el => el.IsClosed)
                    .SelectMany(course => course.Course.Modules)
                    .SelectMany(module => module.Lessons)
                    .Count();

                var completedCompetitionsCount = user.Competitions.Where(competition => competition.Competition.Status == ActiveStatus.Ended).Count();

                var coursesByCategory = user.Courses
                    .Select(course => course.Course)
                    .GroupBy(course => course.Category)
                    .Select((group, i) => new CourseByCategory
                    {
                        Id = i,
                        label = group.Key,
                        value = group.Count() 
                    })
                    .ToList();

                var scoreByMonth = new StatisticHorizontalBar
                {
                    dataset = user.Competitions
                        .Where(c => c.Competition.Status == Competition.ActiveStatus.Ended)
                        .SelectMany(c => c.Competition.Members)
                        .Where(cm => cm.AthleteId == user.Id && cm.Result.HasValue) 
                        .GroupBy(cm => cm.Competition.DateTime.ToString("yyyy-MM"))
                        .Select(group => new StatisticByWeaphon
                        {
                            month = group.Key,
                            avgresult = (int)group.Average(cm => cm.Result.Value)
                        })
                        .ToList()
                };

                return Results.Json(new AnalyticResponse()
                {
                    completedCoursesCount = completedCoursesCount,
                    completedLessonsCount = completedLessonsCount,
                    completedCompetitions = completedCompetitionsCount,
                    coursesByCategory = coursesByCategory,
                    scoreByMonth = scoreByMonth
                });
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 400);
            }
        }
    }
}
