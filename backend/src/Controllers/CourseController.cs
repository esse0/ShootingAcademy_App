using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Competition;
using ShootingAcademy.Models.Controllers.Course;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IResult> Get()
        {
            IEnumerable<Course> Courses = await _context.Courses.ToListAsync();

            return Results.Json(Courses.Select(course =>
            {
                return new CourseModel()
                {
                    Id = course.Id.ToString(),
                    category = course.Category,
                    description = course.Description,
                    duration = course.Duration,
                    level = course.Level,
                    rate = course.Rate,
                    title = course.Title,
                };
            }));
        }

        [HttpGet("user"), Authorize]
        public async Task<IResult> GetUserCourses([FromQuery] bool history = false)
        {
            try
            {
                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;
                Random rand = new();

                var user = await _context.Users
                    .Include(u => u.Courses)
                    .ThenInclude(cm => cm.Course)
                    .FirstAsync(u => u.Id == userGuid);

                var courseBannerTypes = user.Courses
                .Where(cm => history ? cm.IsClosed : !cm.IsClosed)
                .Select(cm => new MyCourseBannerType
                {
                    completed_percent = rand.Next(0, 100),
                    duration = cm.Course.Duration,
                    finished_at = cm.FinishedAt.ToString(),
                    started_at = cm.StartedAt.ToString(),
                    id = cm.Course.Id.ToString(),
                    is_closed = cm.IsClosed,
                    level = cm.Course.Level,
                    title = cm.Course.Title,
                    category = cm.Course.Category,
                })
                .ToList();

                return Results.Json(courseBannerTypes);
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

        [HttpGet("fulldata")]
        public async Task<IResult> GetCourseFullData([FromQuery] string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid cguid))
                    throw new BaseException("Invalid course ID format", 400);

                var courseData = await _context.Courses
                    .AsNoTracking()
                    .Where(c => c.Id == cguid)
                    .Select(course => new CourseModel
                    {
                        Id = course.Id.ToString(),
                        category = course.Category,
                        description = course.Description,
                        duration = course.Duration,
                        title = course.Title,
                        is_closed = course.IsClosed,
                        level = course.Level,
                        peopleRateCount = course.PeopleRateCount,
                        rate = course.Rate,
                        instructor = FullUserModel.FromEntity(course.Instructor),

                        modules = course.Modules
                        .OrderBy(module => module.Oder)
                        .Select(module => new ModuleModel
                        {
                            id = module.Id.ToString(),
                            title = module.Title,
                            lessons = module.Lessons
                                .OrderBy(lesson => lesson.Oder)
                                .Select(lesson => new LessonModel
                                {
                                    id = lesson.Id.ToString(),
                                    description = lesson.Description,
                                    title = lesson.Title,
                                    videoLink = lesson.VideoLink
                                }).ToList()
                        }).ToList(),

                        faqs = course.Faqs
                            .Select(faq => new FaqModel
                            {
                                id = faq.Id.ToString(),
                                answer = faq.Answer,
                                question = faq.Question
                            }).ToList(),

                        features = course.Features
                            .Select(feature => new FeatureModel
                            {
                                id = feature.Id.ToString(),
                                description = feature.Description,
                                title = feature.Title
                            }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (courseData == null)
                    throw new BaseException("Course not found", 404);

                return Results.Json(courseData);
            }
            catch (BaseException exp)
            {
                return Results.Json(exp.GetModel(), statusCode: exp.Code);
            }
        }

        [HttpPost("subscribe"), Authorize]
        public async Task<IResult> SubscribeUser([FromQuery] string courseId)
        {
            try
            {
                if (!Guid.TryParse(courseId, out Guid cguid))
                    throw new BaseException("Invalid course ID format", 400);

                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                bool userAlreadySubscribed = await _context.CourseMembers
                    .AnyAsync(cm => cm.CourseId == cguid && cm.UserId == userGuid);

                if (userAlreadySubscribed)
                    throw new BaseException("The user is already on the course", 400);

                Course? course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Id == cguid);

                if (course == null)
                    throw new BaseException($"Could not find course with ID {courseId}", 404);

                _context.CourseMembers.Add(new CourseMember
                {
                    CourseId = cguid,
                    UserId = userGuid,
                    IsClosed = false,
                    CompletedLessons = new List<Guid>(),
                    StartedAt = DateTime.UtcNow,
                    FinishedAt = null
                });

                await _context.SaveChangesAsync();

                return Results.Ok();
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

        [HttpPut("leaveCourse"), Authorize]
        public async Task<IResult> UnsubscribeUser([FromQuery] string courseId)
        {
            try
            {
                if (!Guid.TryParse(courseId, out Guid cguid))
                    throw new BaseException("Invalid course ID format", 400);

                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var courseMember = await _context.CourseMembers
                    .FirstOrDefaultAsync(cm => cm.CourseId == cguid && cm.UserId == userGuid);

                if (courseMember == null)
                    throw new BaseException("The user is not on the course", 404);

                courseMember.IsClosed = true;
                courseMember.FinishedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Results.Ok();
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


        [HttpGet("admin"), Authorize(Roles = "moderator")]
        public async Task<IResult> GetAdminCourses()
        {
            try
            {
                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var courses = await _context.Courses
                    .Where(c => c.InstructorId == userGuid)
                    .Include(c => c.Modules) 
                    .Include(c => c.Members)
                    .AsNoTracking()
                    .ToListAsync();
                
                var coursesType = courses.Select(c => new CourseModel()
                {
                    Id = c.Id.ToString(),
                    category = c.Category,
                    description = c.Description,
                    duration = c.Duration,
                    level = c.Level,
                    rate = c.Rate,
                    title = c.Title,
                }).ToList();

                return Results.Json(coursesType);
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

        [HttpPost("create"), Authorize(Roles = "moderator")]
        public async Task<IResult> CreateCourse([FromBody] CourseModel course)
        {
            Random random = new Random();

            try
            {
                if (string.IsNullOrWhiteSpace(course.title) ||
                    string.IsNullOrWhiteSpace(course.description) ||
                    string.IsNullOrWhiteSpace(course.duration))
                {
                    throw new BaseException("Invalid input: required fields are missing.", 400);
                }

                Guid instructorId = AutorizeData.FromContext(HttpContext).UserGuid;

                bool courseExists = await _context.Courses
                    .AsNoTracking()
                    .AnyAsync(c => c.Title == course.title && c.InstructorId == instructorId);

                if (courseExists)
                    throw new BaseException("A course with this title already exists.", 409);

                int randomNumber = random.Next(10, 501);

                var newCourse = new Course
                {
                    Id = Guid.NewGuid(),
                    Title = course.title,
                    Description = course.description,
                    Duration = course.duration,
                    Level = course.level ?? "Beginner",
                    Category = course.category ?? "General",
                    IsClosed = course.is_closed ?? false,
                    InstructorId = instructorId,
                    Rate = course.rate,
                    PeopleRateCount = randomNumber
                };

                if(course.modules == null || course.modules.Count == 0)
                {
                    throw new BaseException("Modules is empty", 400);
                }

                foreach (var module in course.modules) {
                    if (module.lessons == null || module.lessons.Count == 0)
                    {
                        throw new BaseException("Modules is empty", 400);
                    }
                }

                if (course.modules != null && course.modules.Any())
                {
                    newCourse.Modules = course.modules.Select((m, moduleIndex) => new Module
                    {
                        Id = Guid.NewGuid(),
                        Title = m.title,
                        CourseId = newCourse.Id,
                        Oder = moduleIndex + 1,
                        Lessons = m.lessons?.Select((l, lessonIndex) => new Lesson
                        {
                            Id = Guid.NewGuid(),
                            Title = l.title,
                            Description = l.description,
                            VideoLink = l.videoLink,
                            Oder = lessonIndex + 1
                        }).ToList() ?? new List<Lesson>()
                    }).ToList();
                }

                if (course.faqs != null && course.faqs.Any())
                {
                    newCourse.Faqs = course.faqs.Select(f => new Faq
                    {
                        Id = Guid.NewGuid(),
                        Question = f.question,
                        Answer = f.answer,
                        CourseId = newCourse.Id
                    }).ToList();
                }

                if (course.features != null && course.features.Any())
                {
                    newCourse.Features = course.features.Select(f => new Feature
                    {
                        Id = Guid.NewGuid(),
                        Title = f.title,
                        Description = f.description,
                        CourseId = newCourse.Id
                    }).ToList();
                }

                _context.Courses.Add(newCourse);

                await _context.SaveChangesAsync();

                return Results.StatusCode(201);
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        } 

        [HttpDelete("delete"), Authorize(Roles = "moderator")]
        public async Task<IResult> DeleteCourse([FromQuery] string courseId)
        {
            try
            {
                if (!Guid.TryParse(courseId, out Guid courseGuid))
                {
                    throw new BaseException("Invalid course ID format.", code: 400);
                }

                Guid userGuid = AutorizeData.FromContext(HttpContext).UserGuid;

                var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseGuid);

                if (course == null)
                {
                    throw new BaseException("Course not found.", code: 404);
                }

                if (course.InstructorId != userGuid)
                {
                    throw new BaseException("Unauthorized", code: 401);
                }

                _context.Courses.Remove(course);

                await _context.SaveChangesAsync();

                return Results.Ok();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 500);
            }
        }

    }
}
