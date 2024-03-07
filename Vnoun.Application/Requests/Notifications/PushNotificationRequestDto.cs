using System.ComponentModel.DataAnnotations;

namespace Vnoun.Application.Requests.Notifications;

public class PushNotificationRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Link is required")]
    public string Link { get; set; }
}