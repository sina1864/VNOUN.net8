using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Core.Repositories.Dtos.Requests.Notifications;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public async Task DeleteAllMyNotification(string userId)
    {
        var filter = Builders<Notification>.Filter.Eq("user", MongoDB.Bson.ObjectId.Parse(userId));
        await DB.Collection<Notification>().DeleteManyAsync(filter);
    }

    public async Task<List<Notification>> GetAllMyNotification(string userId, string? queryString = null)
    {
        if (queryString != null)
        {
            IMongoQueryable<Notification> query = DB.Collection<Notification>().AsQueryable();

            var features = new APIFeatures<Notification>(query, queryString);
            var notifs = await features.Filter()
                                .Sort()
                                .Paginate()
                                .GetQuery()
                                .Where(x => x.UserId.ToString() == userId)
                                .ToListAsync();

            foreach (var notification in notifs)
            {
                var user = await DB.Find<User>()
                    .Match(u => u.ID == notification.UserId.ToString())
                    .ExecuteFirstAsync();

                notification.User = user;
            }

            return notifs;
        }

        var notifications = await DB.Find<Notification>()
            .Match(u => u.UserId.ToString() == userId)
            .ExecuteAsync();

        foreach (var notification in notifications)
        {
            var user = await DB.Find<User>()
                .Match(u => u.ID == notification.UserId.ToString())
                .ExecuteFirstAsync();

            notification.User = user;
        }

        return notifications;
    }

    public async Task MakeAllAsRead()
    {
        await DB.Update<Notification>()
            .Match(n => n.Seen == false)
            .Modify(b => b.Set(n => n.Seen, true))
            .ExecuteAsync();
    }

    public async Task<Notification?> PushNotification(string userId, PushNotificationServiceRequestDto requestDto)
    {
        Notification toCreate = new()
        {
            Title = requestDto.Title,
            Description = requestDto.Description,
            UserId = MongoDB.Bson.ObjectId.Parse(userId),
            Link = requestDto.Link
        };

        await CreateAsync(toCreate);

        return await FindById(toCreate.ID);
    }

    public async Task<List<Notification>> PushNotificationForMultipleUsers(List<string> users, PushNotificationServiceRequestDto requestDto)
    {
        return await Task.Run(async () =>
        {
            List<Notification> notifications = new();
            foreach (var user in users)
            {
                try
                {
                    var notif = await PushNotification(user, requestDto);
                    if (notif != null)
                    {
                        notifications.Add(notif);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return notifications;
        });
    }
}