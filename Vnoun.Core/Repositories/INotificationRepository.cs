using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;
using Vnoun.Core.Repositories.Dtos.Requests.Notifications;

namespace Vnoun.Core.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetAllMyNotification(string userId, string? queryString = null);
    Task MakeAllAsRead();
    Task DeleteAllMyNotification(string userId);
    Task<Notification?> PushNotification(string userId, PushNotificationServiceRequestDto requestDto);
    Task<List<Notification>> PushNotificationForMultipleUsers(List<string> users, PushNotificationServiceRequestDto requestDto);
}