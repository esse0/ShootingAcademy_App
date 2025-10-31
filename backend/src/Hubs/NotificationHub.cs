using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShootingAcademy.Models.Controllers.Notification;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services;

namespace ShootingAcademy.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        private readonly IGroupService _groupService;
        private readonly IOrganizationService _organizationService;

        public NotificationHub(INotificationService notificationService, IGroupService groupService, IOrganizationService organizationService)
        {
            _notificationService = notificationService;
            _groupService = groupService;
            _organizationService = organizationService;
        }

        private Guid GetUserId()
        {
            var authData = AutorizeData.FromContext(Context.GetHttpContext());
            if (!authData.IsAutorize)
            {
                throw new HubException("Пользователь не авторизован");
            }
            return authData.UserGuid;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = GetUserId();

                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnConnectedAsync: {ex.Message}");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                Console.WriteLine($"Connection closed: {Context.ConnectionId}");
                if (exception != null)
                {
                    Console.WriteLine($"Disconnect reason: {exception.Message}");
                }
                var userId = GetUserId();
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnDisconnectedAsync: {ex.Message}");
                throw;
            }
        }

        public async Task SendNotification(NotificationDTO notification)
        {
            await Clients.Group(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification);
        }

        public async Task InviteToGroup(Guid groupId, Guid invitedUserId, string message)
        {
            var userId = GetUserId();

            try
            {
                var group = await _groupService.GetGroupByIdAsync(groupId);

                if (group == null)
                {
                    throw new HubException("Группа не найдена");
                }

                if (group.coach.Id != userId)
                {
                    throw new HubException("Только лидер группы может отправлять приглашения");
                }

                var notification = new CreateNotificationDTO
                {
                    UserId = invitedUserId,
                    SenderId = userId,
                    Type = NotificationType.GroupInvitation,
                    Title = "Приглашение в группу",
                    Message = message,
                    RequiresResponse = true,
                    InviteId = groupId,
                };

                await _groupService.InviteMemberAsync(groupId, invitedUserId, userId);

                var createdNotification = await _notificationService.CreateNotificationAsync(notification);

                await SendNotification(createdNotification);
                await Clients.Group(userId.ToString()).SendAsync("UpdateGroupInviteMembers");
            }
            catch (BaseException ex)
            {
                throw new HubException($"Ошибка: {ex.Message}");
            }
            catch
            {
                throw new HubException($"Ошибка: низвестная ошибка");
            }
        }

        public async Task InviteToOrganization(Guid organizationId, Guid invitedUserId, string message)
        {
            var userId = GetUserId();

            try
            {
                var organization = await _organizationService.GetByIdAsync(organizationId);

                if (organization == null)
                {
                    throw new HubException("Организация не найдена");
                }

                var notification = new CreateNotificationDTO
                {
                    UserId = invitedUserId,
                    SenderId = userId,
                    Type = NotificationType.OrganizationInvitation,
                    Title = "Приглашение в организацию",
                    Message = message,
                    RequiresResponse = true,
                    InviteId = organizationId
                };

                await _organizationService.InviteMemberAsync(organizationId, invitedUserId, userId);

                var createdNotification = await _notificationService.CreateNotificationAsync(notification);

                await SendNotification(createdNotification);
                await Clients.Group(userId.ToString()).SendAsync("UpdateOrganizationInviteMembers");
            }
            catch (BaseException ex)
            {
                throw new HubException($"Ошибка: {ex.Message}");
            }
            catch
            {
                throw new HubException($"Ошибка: низвестная ошибка");
            }
        }

        public async Task RespondToInvitation(Guid notificationId, bool accept, string responseMessage = null)
        {
            var userId = GetUserId();
            try
            {
                var notification = await _notificationService.GetNotificationAsync(notificationId, userId);

                if (notification == null)
                    throw new HubException("Уведомление не найдено");

                if (notification.IsRead || !string.IsNullOrEmpty(notification.Response))
                    throw new HubException("На это приглашение уже был дан ответ");

                if (notification.InviteId == null)
                    throw new HubException("Уведомление не содержит InviteId");
                if (notification.SenderId == null)
                    throw new HubException("Уведомление не содержит SenderId");

                var updateDto = new UpdateNotificationDTO
                {
                    IsRead = true,
                    Response = responseMessage ?? (accept ? "Принято" : "Отклонено")
                };

                switch (notification.Type)
                {
                    case NotificationType.OrganizationInvitation:
                        {
                            await _organizationService.UpdateStatusMemberAsync(Guid.Parse(notification.InviteId), userId, Guid.Parse(notification.SenderId), accept ? OrganizationMemberStatus.Approved : OrganizationMemberStatus.Rejected);
                            break;
                        }
                    case NotificationType.GroupInvitation:
                        {
                            await _groupService.UpdateStatusMemberAsync(Guid.Parse(notification.InviteId), userId, Guid.Parse(notification.SenderId), accept ? GroupMemberStatus.Approved : GroupMemberStatus.Rejected);
                            break;
                        }
                }

                var updatedNotification = await _notificationService.UpdateNotificationAsync(notificationId, updateDto, userId);

                if (notification.SenderId != null)
                {
                    var responseNotification = new CreateNotificationDTO
                    {
                        UserId = Guid.Parse(notification.SenderId),
                        SenderId = userId,
                        Type = NotificationType.Message,
                        Title = $"Ответ на запрос: {notification.Title}",
                        Message = $"Ваш запрос был {(accept ? "принят" : "отклонен")}. {responseMessage}",
                        RequiresResponse = false
                    };

                    var createdResponseNotification = await _notificationService.CreateNotificationAsync(responseNotification);
                    await SendNotification(createdResponseNotification);

                    // Отправляем событие обновления списка участников тренеру
                    if(notification.Type == NotificationType.GroupInvitation) await Clients.Group(notification.SenderId.ToString()).SendAsync("UpdateGroupInviteMembers");
                    else if(notification.Type == NotificationType.OrganizationInvitation) await Clients.Group(notification.SenderId.ToString()).SendAsync("UpdateOrganizationInviteMembers");
                }

                await SendNotification(updatedNotification);

                await Clients.Group(userId.ToString()).SendAsync("UpdateNotifications");
            }
            catch (BaseException ex)
            {
                throw new HubException($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RespondToInvitation] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                
                throw new HubException("Ошибка: низвестная ошибка");
            }
        }

        public async Task CancelInvitation(Guid groupId, Guid invitedUserId)
        {
            var userId = GetUserId();
            try
            {
                var group = await _groupService.GetGroupByIdAsync(groupId);

                if (group == null)
                {
                    throw new HubException("Группа не найдена");
                }

                if (group.coach.Id.ToString() != userId.ToString())
                {
                    throw new HubException("Только лидер группы может отменять приглашения");
                }

                await _groupService.KickMemberAsync(groupId.ToString(), invitedUserId.ToString(), userId);

                var existingNotification = await _notificationService.GetNotificationByInviteAndUser(groupId, userId, invitedUserId) ?? throw new HubException($"Уведомление для отмены не найдено");

                var updateDto = new UpdateNotificationDTO
                {
                    IsRead = true,
                    Response = "Приглашение отменено"
                };

                var updatedNotification = await _notificationService.UpdateNotificationAsync(Guid.Parse(existingNotification.Id), updateDto, invitedUserId);

                await SendNotification(updatedNotification);

                await Clients.Group(userId.ToString()).SendAsync("UpdateGroupInviteMembers");
            }
            catch (BaseException ex)
            {
                throw new HubException($"Ошибка: {ex.Message}");
            }
            catch
            {
                throw new HubException($"Ошибка: неизвестная ошибка");
            }
        }

        public async Task CancelOrganizationInvitation(Guid organizationId, Guid invitedUserId)
        {
            var userId = GetUserId();
            try
            {
                await _organizationService.CancelOrganizationInvitationAsync(organizationId, invitedUserId, userId);

                // Найти уведомление о приглашении
                var existingNotification = await _notificationService.GetNotificationByInviteAndUser(organizationId, userId, invitedUserId)
                    ?? throw new HubException($"Уведомление для отмены не найдено");

                var updateDto = new UpdateNotificationDTO
                {
                    IsRead = true,
                    Response = "Приглашение отменено"
                };

                var updatedNotification = await _notificationService.UpdateNotificationAsync(Guid.Parse(existingNotification.Id), updateDto, invitedUserId);

                // Отправить обновлённое уведомление коучу
                await SendNotification(updatedNotification);

                // Обновить таблицу у владельца
                await Clients.Group(userId.ToString()).SendAsync("UpdateOrganizationInviteMembers");
                // Обновить таблицу у приглашённого коуча
                await Clients.Group(invitedUserId.ToString()).SendAsync("UpdateOrganizationInviteMembers");
            }
            catch (BaseException ex)
            {
                throw new HubException($"Ошибка: {ex.Message}");
            }
            catch
            {
                throw new HubException($"Ошибка: неизвестная ошибка");
            }
        }
    }
}
