using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Services;

namespace ShootingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return HandleResult(notifications);
        }

        [HttpPost("{notificationId}/accept"), Authorize]
        public async Task<IActionResult> AcceptInvitation(string notificationId)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var notification = await _notificationService.AcceptInvitationAsync(Guid.Parse(notificationId), userId);
            return HandleResult(notification);
        }

        [HttpPost("{notificationId}/decline"), Authorize]
        public async Task<IActionResult> DeclineInvitation(string notificationId)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var notification = await _notificationService.DeclineInvitationAsync(Guid.Parse(notificationId), userId);
            return HandleResult(notification);
        }
    }
} 