namespace Vnoun.Core.Repositories.Dtos.Requests.Notifications;

public class PushNotificationServiceRequestDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Link { get; set; }
}