using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Course;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services.Media;

namespace ShootingAcademy.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseModel>> GetAllCoursesAsync();
        Task<IEnumerable<MyCourseBannerType>> GetUserCoursesAsync(Guid userId, bool history = false);
        Task<CourseWithVideoModel> GetCourseFullDataAsync(string courseId, Guid userId);
        Task SubscribeUserAsync(string courseId, Guid userId);
        Task UnsubscribeUserAsync(string courseId, Guid userId);
        Task<IEnumerable<CourseModel>> GetAdminCoursesAsync(Guid instructorId);
        Task DeleteCourseAsync(string courseId, Guid instructorId);
        Task<Course> CreateCourseAsync(CourseModel? course, Guid instructorId);
        Task MarkLessonAsCompletedAsync(string courseId, string lessonId, Guid userId);
        Task UnmarkLessonAsCompletedAsync(string courseId, string lessonId, Guid userId);
        Task<CourseProgressModel> GetCourseProgressAsync(string courseId, Guid userId);
    }

    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IVideoService _videoService;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ApplicationDbContext context, IVideoService videoService, ILogger<CourseService> logger)
        {
            _context = context;
            _videoService = videoService;
            _logger = logger;
        }

        public async Task<IEnumerable<CourseModel>> GetAllCoursesAsync()
        {
            var courses = await _context.Courses.ToListAsync();

            return courses.Select(course => new CourseModel()
            {
                Id = course.Id.ToString(),
                category = course.Category,
                description = course.Description,
                duration = course.Duration,
                level = course.Level,
                rate = course.Rate,
                title = course.Title,
            });
        }

        public async Task<IEnumerable<MyCourseBannerType>> GetUserCoursesAsync(Guid userId, bool history = false)
        {
            var user = await _context.Users
                .Include(u => u.Courses)
                    .ThenInclude(cm => cm.Course)
                        .ThenInclude(c => c.Modules)
                            .ThenInclude(m => m.Lessons)
                .FirstAsync(u => u.Id == userId);

            return user.Courses
                .Where(cm => history ? cm.IsClosed : !cm.IsClosed)
                .Select(cm => {
                    var totalLessons = cm.Course.Modules.Sum(m => m.Lessons.Count);
                    var completedLessons = cm.CompletedLessons.Count;
                    var progress = totalLessons > 0 ? (double)completedLessons / totalLessons * 100 : 0;

                    return new MyCourseBannerType
                    {
                        completed_percent = (int)Math.Round(progress, 2),
                        duration = cm.Course.Duration,
                        finished_at = cm.FinishedAt?.ToString() ?? "",
                        started_at = cm.StartedAt.ToString(),
                        id = cm.Course.Id.ToString(),
                        is_closed = cm.IsClosed,
                        level = cm.Course.Level,
                        title = cm.Course.Title,
                        category = cm.Course.Category,
                    };
                })
                .ToList();
        }

        public async Task<CourseWithVideoModel> GetCourseFullDataAsync(string courseId, Guid userId)
        {
            if (!Guid.TryParse(courseId, out Guid cguid))
                throw new BaseException("Invalid course ID format", 400);

            var course = await _context.Courses
                .Where(c => c.Id == cguid)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                        .ThenInclude(l => l.LessonVideo)
                            .ThenInclude(lv => lv.Media)
                .Include(c => c.Faqs)
                .Include(c => c.Features)
                .Include(c => c.Instructor)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (course == null)
                throw new BaseException("Course not found", 404);

            var courseMember = await _context.CourseMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(cm => cm.CourseId == cguid && cm.UserId == userId);

            var completedLessons = courseMember?.CompletedLessons ?? new List<Guid>();
            _logger.LogInformation($"Retrieved completed lessons for user {userId} in course {courseId}: {string.Join(", ", completedLessons)}");

            var courseModel = new CourseWithVideoModel
            {
                Id = course.Id.ToString(),
                category = course.Category,
                description = course.Description,
                duration = course.Duration,
                title = course.Title,
                is_closed = courseMember?.IsClosed ?? false,
                level = course.Level,
                peopleRateCount = course.PeopleRateCount,
                rate = course.Rate,
                instructor = FullUserModel.FromEntity(course.Instructor),
                modules = course.Modules
                    .OrderBy(m => m.Oder)
                    .Select(m => new ModuleWithVideoModel
                    {
                        id = m.Id.ToString(),
                        title = m.Title,
                        lessons = m.Lessons
                            .OrderBy(l => l.Oder)
                            .Select(l => new LessonWithVideoModel
                            {
                                id = l.Id.ToString(),
                                title = l.Title,
                                description = l.Description,
                                videoId = l.LessonVideo?.MediaId.ToString(),
                                isCompleted = completedLessons.Contains(l.Id)
                            }).ToList()
                    }).ToList(),
                faqs = course.Faqs.Select(f => new FaqModel
                {
                    id = f.Id.ToString(),
                    answer = f.Answer,
                    question = f.Question
                }).ToList(),
                features = course.Features.Select(f => new FeatureModel
                {
                    id = f.Id.ToString(),
                    title = f.Title,
                    description = f.Description
                }).ToList()
            };

            foreach (var module in courseModel.modules)
            {
                foreach (var lesson in module.lessons)
                {
                    if (!string.IsNullOrEmpty(lesson.videoId))
                    {
                        _logger.LogInformation($"Getting video URL for lesson {lesson.id} with videoId {lesson.videoId}");
                        try 
                        {
                            var uri = await _videoService.GetFileUrl(Guid.Parse(lesson.videoId));
                            _logger.LogInformation($"Received video URL: {uri.FileUri}");
                            lesson.videoUri = uri.FileUri;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error getting video URL for lesson {lesson.id}: {ex.Message}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"No videoId for lesson {lesson.id}");
                    }
                }
            }

            return courseModel;
        }


        public async Task SubscribeUserAsync(string courseId, Guid userId)
        {
            if (!Guid.TryParse(courseId, out Guid cguid))
                throw new BaseException("Invalid course ID format", 400);

            bool userAlreadySubscribed = await _context.CourseMembers
                .AnyAsync(cm => cm.CourseId == cguid && cm.UserId == userId);

            if (userAlreadySubscribed)
                throw new BaseException("The user is already on the course", 400);

            Course? course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == cguid);

            if (course == null)
                throw new BaseException($"Could not find course with ID {courseId}", 404);

            _context.CourseMembers.Add(new CourseMember
            {
                CourseId = cguid,
                UserId = userId,
                IsClosed = false,
                CompletedLessons = new List<Guid>(),
                StartedAt = DateTime.UtcNow,
                FinishedAt = null
            });

            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeUserAsync(string courseId, Guid userId)
        {
            if (!Guid.TryParse(courseId, out Guid cguid))
                throw new BaseException("Invalid course ID format", 400);

            var courseMember = await _context.CourseMembers
                .FirstOrDefaultAsync(cm => cm.CourseId == cguid && cm.UserId == userId);

            if (courseMember == null)
                throw new BaseException("The user is not on the course", 404);

            if (courseMember.FinishedAt != null)
                throw new BaseException("Course already ended", 400);

            // Получаем все уроки курса
            var course = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.Id == cguid);

            if (course == null)
                throw new BaseException("Course not found", 404);

            // Получаем все ID уроков курса
            var allLessonIds = course.Modules
                .SelectMany(m => m.Lessons)
                .Select(l => l.Id)
                .ToList();

            // Проверяем, все ли уроки завершены
            var notCompletedLessons = allLessonIds
                .Where(lessonId => !courseMember.CompletedLessons.Contains(lessonId))
                .ToList();

            if (notCompletedLessons.Any())
            {
                throw new BaseException("Not all lessons of the course are completed. Please complete all lessons before completing the course.", 400);
            }

            courseMember.IsClosed = true;
            courseMember.FinishedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseModel>> GetAdminCoursesAsync(Guid instructorId)
        {
            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.Instructor)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons) // ���� ����������� m.Lessons
                .Include(c => c.Faqs)
                .Include(c => c.Features)
                .Include(c => c.Members)
                .AsNoTracking()
                .ToListAsync();

            return courses.Select(c => new CourseModel()
            {
                Id = c.Id.ToString(),
                category = c.Category,
                description = c.Description,
                duration = c.Duration,
                level = c.Level,
                rate = c.Rate,
                title = c.Title,
                is_closed = c.IsClosed,
                peopleRateCount = c.PeopleRateCount,
                instructor = FullUserModel.FromEntity(c.Instructor),
                modules = c.Modules
                    .OrderBy(m => m.Oder)
                    .Select(m => new ModuleModel
                    {
                        id = m.Id.ToString(),
                        title = m.Title,
                        lessons = m.Lessons
                            .OrderBy(l => l.Oder)
                            .Select(l => new LessonModel
                            {
                                id = l.Id.ToString(),
                                description = l.Description,
                                title = l.Title
                            }).ToList()
                    }).ToList(),
                faqs = c.Faqs
                    .Select(f => new FaqModel
                    {
                        id = f.Id.ToString(),
                        answer = f.Answer,
                        question = f.Question
                    }).ToList(),
                features = c.Features
                    .Select(f => new FeatureModel
                    {
                        id = f.Id.ToString(),
                        description = f.Description,
                        title = f.Title
                    }).ToList()
            }).ToList();
        }

        public async Task DeleteCourseAsync(string courseId, Guid instructorId)
        {
            if (!Guid.TryParse(courseId, out Guid cguid))
                throw new BaseException("Invalid course ID format", 400);

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == cguid && c.InstructorId == instructorId);

            if (course == null)
                throw new BaseException("Course not found or you don't have permission to delete it", 404);

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<Course> CreateCourseAsync(CourseModel? course, Guid instructorId)
        {
            if (course == null)
                throw new BaseException("Course not be null", 400);

            if (string.IsNullOrWhiteSpace(course.title) ||
                string.IsNullOrWhiteSpace(course.description) ||
                string.IsNullOrWhiteSpace(course.duration))
                throw new BaseException("Invalid input: required fields are missing.", 400);

            bool courseExists = await _context.Courses.AnyAsync(c => c.Title == course.title && c.InstructorId == instructorId);
            if (courseExists)
                throw new BaseException("A course with this title already exists.", 409);

            if (course.modules == null || course.modules.Count == 0)
                throw new BaseException("Modules is empty", 400);

            foreach (var module in course.modules)
            {
                if (module.lessons == null || module.lessons.Count == 0)
                    throw new BaseException("Modules is empty", 400);
            }

            Random random = new Random();
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
                PeopleRateCount = randomNumber,
                Modules = new List<Module>(),
                Faqs = new List<Faq>(),
                Features = new List<Feature>()
            };

            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            //    
            foreach (var (moduleDto, moduleIndex) in course.modules.Select((m, i) => (m, i)))
            {
                _logger.LogInformation($"Processing module: {moduleDto.title}");
                
                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    Title = moduleDto.title,
                    CourseId = newCourse.Id,
                    Oder = moduleIndex + 1,
                    Lessons = new List<Lesson>()
                };

                _context.Modules.Add(module);
                await _context.SaveChangesAsync();

                foreach (var (lessonDto, lessonIndex) in moduleDto.lessons.Select((l, i) => (l, i)))
                {
                    _logger.LogInformation($"Processing lesson: {lessonDto.title}, videoId: {lessonDto.videoId}");
                    
                    var lesson = new Lesson
                    {
                        Id = Guid.NewGuid(),
                        Title = lessonDto.title,
                        Description = lessonDto.description,
                        Oder = lessonIndex + 1,
                        ModuleId = module.Id
                    };

                    _context.Lessons.Add(lesson);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Created lesson with ID: {lesson.Id}");

                    if (!string.IsNullOrWhiteSpace(lessonDto.videoId))
                    {
                        _logger.LogInformation($"VideoId is not empty: {lessonDto.videoId}");
                        
                        if (Guid.TryParse(lessonDto.videoId, out Guid videoIdGuid))
                        {
                            _logger.LogInformation($"Successfully parsed videoId to GUID: {videoIdGuid}");
                            
                            var media = await _context.MediaStorages.FindAsync(videoIdGuid);
                            if (media != null)
                            {
                                _logger.LogInformation($"Found media storage for videoId: {videoIdGuid}");
                                
                                try 
                                {
                                    var lessonVideo = new LessonVideo
                                    {
                                        Id = Guid.NewGuid(),
                                        Title = lesson.Title,
                                        LessonId = lesson.Id,
                                        Lesson = lesson,
                                        MediaId = media.Id,
                                        Media = media
                                    };
                                    
                                    _logger.LogInformation($"Created LessonVideo with ID: {lessonVideo.Id}");
                                    
                                    _context.LessonVideos.Add(lessonVideo);
                                    await _context.SaveChangesAsync();
                                    
                                    lesson.LessonVideo = lessonVideo;
                                    _logger.LogInformation($"Added LessonVideo to context and linked to lesson");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error creating LessonVideo: {ex.Message}");
                                    throw;
                                }
                            }
                            else
                            {
                                _logger.LogWarning($"Media storage not found for videoId: {videoIdGuid}");
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to parse videoId to GUID: {lessonDto.videoId}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"No videoId provided for lesson: {lessonDto.title}");
                    }

                    module.Lessons.Add(lesson);
                }

                newCourse.Modules.Add(module);
            }

            //  FAQs
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

            //  Features
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

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Course created successfully with ID: {newCourse.Id}");

            return newCourse;
        }

        public async Task MarkLessonAsCompletedAsync(string courseId, string lessonId, Guid userId)
        {
            if (!Guid.TryParse(courseId, out Guid cguid) || !Guid.TryParse(lessonId, out Guid lguid))
                throw new CourseProgressValidationException("Неверный формат ID");

            _logger.LogInformation($"Marking lesson {lessonId} as completed for user {userId} in course {courseId}");

            var courseMember = await _context.CourseMembers
                .FirstOrDefaultAsync(cm => cm.CourseId == cguid && cm.UserId == userId);

            if (courseMember == null)
                throw new CourseProgressNotFoundException(courseId, userId);

            if (courseMember.IsClosed)
                throw new CourseProgressValidationException("Курс уже завершен");

            // Проверяем, существует ли урок в курсе
            var lessonExists = await _context.Lessons
                .AnyAsync(l => l.Id == lguid && l.Module.CourseId == cguid);

            if (!lessonExists)
                throw new LessonNotFoundException(lessonId, courseId);

            _logger.LogInformation($"Current completed lessons: {string.Join(", ", courseMember.CompletedLessons)}");

            // Добавляем урок в список пройденных, если его там еще нет
            if (!courseMember.CompletedLessons.Contains(lguid))
            {
                courseMember.CompletedLessons.Add(lguid);
                _context.CourseMembers.Update(courseMember);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Lesson {lessonId} marked as completed. New completed lessons: {string.Join(", ", courseMember.CompletedLessons)}");
            }
            else
            {
                _logger.LogInformation($"Lesson {lessonId} is already marked as completed");
            }
        }

        public async Task UnmarkLessonAsCompletedAsync(string courseId, string lessonId, Guid userId)
        {
            if (!Guid.TryParse(courseId, out Guid cguid) || !Guid.TryParse(lessonId, out Guid lguid))
                throw new CourseProgressValidationException("Неверный формат ID");

            _logger.LogInformation($"Unmarking lesson {lessonId} as completed for user {userId} in course {courseId}");

            var courseMember = await _context.CourseMembers
                .FirstOrDefaultAsync(cm => cm.CourseId == cguid && cm.UserId == userId);

            if (courseMember == null)
                throw new CourseProgressNotFoundException(courseId, userId);

            if (courseMember.IsClosed)
                throw new CourseProgressValidationException("Курс уже завершен");

            // Проверяем, существует ли урок в курсе
            var lessonExists = await _context.Lessons
                .AnyAsync(l => l.Id == lguid && l.Module.CourseId == cguid);

            if (!lessonExists)
                throw new LessonNotFoundException(lessonId, courseId);

            _logger.LogInformation($"Current completed lessons: {string.Join(", ", courseMember.CompletedLessons)}");

            // Удаляем урок из списка пройденных, если он там есть
            if (courseMember.CompletedLessons.Contains(lguid))
            {
                courseMember.CompletedLessons.Remove(lguid);
                _context.CourseMembers.Update(courseMember);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Lesson {lessonId} unmarked as completed. New completed lessons: {string.Join(", ", courseMember.CompletedLessons)}");
            }
            else
            {
                _logger.LogInformation($"Lesson {lessonId} is not marked as completed");
            }
        }

        public async Task<CourseProgressModel> GetCourseProgressAsync(string courseId, Guid userId)
        {
            if (!Guid.TryParse(courseId, out Guid cguid))
                throw new CourseProgressValidationException("Неверный формат ID курса");

            var courseMember = await _context.CourseMembers
                .FirstOrDefaultAsync(cm => cm.CourseId == cguid && cm.UserId == userId);

            if (courseMember == null)
                throw new CourseProgressNotFoundException(courseId, userId);

            var course = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.Id == cguid);

            if (course == null)
                throw new CourseProgressNotFoundException(courseId, userId);

            var totalLessons = course.Modules.Sum(m => m.Lessons.Count);
            var completedLessons = courseMember.CompletedLessons.Count;
            var progress = totalLessons > 0 ? (double)completedLessons / totalLessons * 100 : 0;

            return new CourseProgressModel
            {
                TotalLessons = totalLessons,
                CompletedLessons = completedLessons,
                ProgressPercentage = Math.Round(progress, 2),
                CompletedLessonIds = courseMember.CompletedLessons.Select(id => id.ToString()).ToList()
            };
        }
    }
} 