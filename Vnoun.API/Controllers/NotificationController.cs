using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Notifications;
using Vnoun.Application.Responses.Notifications;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Core.Repositories.Dtos.Requests.Notifications;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/notification")]
public class NotificationController : BaseController<Notification>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public NotificationController(IMapper mapper, IWebHostEnvironment hostingEnvironment, INotificationRepository notificationRepository, IUserRepository userRepository) : base(hostingEnvironment)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetMyNotifications([FromQuery] string? query, [FromQuery] string? sort, [FromQuery] int? limit, [FromQuery] int? skip)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        var queryDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(query ?? "{}");

        if (limit != null)
            queryDict.Add("limit", limit.ToString());

        if (skip != null)
            queryDict.Add("skip", skip.ToString());

        if (sort != null)
            queryDict.Add("sort", sort);

        query = Newtonsoft.Json.JsonConvert.SerializeObject(queryDict);

        var notifications = await _notificationRepository.GetAllMyNotification(userId, query);

        var notificationsDto = _mapper.Map<List<NotificationResponseWithUser>>(notifications);

        return Ok(new
        {
            status = "success",
            results = notificationsDto.Count,
            data = notificationsDto
        });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> MakeAsRead(string id)
    {
        var userId = GetUserIdFromJsonWebToken();

        var notification = await _notificationRepository.FindById(id);
        if (notification == null)
        {
            return NotFound(new
            {
                status = "error",
                message = "Notification not found"
            });
        }

        if (notification.UserId.ToString() != userId)
        {
            return Unauthorized(new
            {
                status = "error",
                message = "Unauthorized"
            });
        }

        notification.Seen = true;
        var updated = await _notificationRepository.UpdateOneAsync(notification.ID, notification);

        return Ok(new
        {
            status = "success",
        });
    }

    [HttpPatch("make-all-as-read")]
    public async Task<IActionResult> MakeAllAsRead()
    {
        var userId = GetUserIdFromJsonWebToken();
        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
        {
            return Unauthorized(new
            {
                status = "error",
                message = "Unauthorized"
            });
        }

        await _notificationRepository.MakeAllAsRead();

        return Ok(new
        {
            status = "success",
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
        {
            return Unauthorized(new
            {
                status = "error",
                message = "Unauthorized"
            });
        }

        var notification = await _notificationRepository.FindById(id);
        if (notification == null)
            throw new AppException("Notification not found", 404);

        await _notificationRepository.DeleteOneAsync(notification.ID);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllMyNotification()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        await _notificationRepository.DeleteAllMyNotification(userId);

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> PushNotificationForOneUser([FromQuery] string to_user_id, [FromBody] PushNotificationRequestDto requestDto)
    {
        if (string.IsNullOrEmpty(to_user_id))
            throw new AppException("to_user_id is required", 400);

        var userId = GetUserIdFromJsonWebToken();
        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var notification = await _notificationRepository.PushNotification(to_user_id, _mapper.Map<PushNotificationServiceRequestDto>(requestDto));
        var notifResponse = _mapper.Map<NotificationResponse>(notification);

        return Ok(new
        {
            status = "success",
            data = notifResponse
        });
    }

    [HttpPost("pushNotificationForMultibleUsers")]
    public async Task<IActionResult> PushNotificationForMultipleUsers([FromBody] MultipleNotificationPushRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var data = await _notificationRepository.PushNotificationForMultipleUsers(requestDto.UserIds, new PushNotificationServiceRequestDto
        {
            Title = requestDto.Title,
            Description = requestDto.Description,
        });

        var responseData = _mapper.Map<List<NotificationResponse>>(data);

        return Ok(new
        {
            status = "success",
            data = responseData
        });
    }
}