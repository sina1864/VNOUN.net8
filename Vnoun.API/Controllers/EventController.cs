using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Event;
using Vnoun.Application.Responses.Event;
using Vnoun.Core.Entities;
using Vnoun.Core.Entities.MetaEntities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventController : BaseController<Event>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public EventController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IEventRepository eventRepository, IUserRepository userRepository) : base(
        hostingEnvironment)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] string? query)
    {
        var events = await _eventRepository.GetAllAsync(query);

        var response = _mapper.Map<List<EventResponseDto>>(events);

        return Ok(new
        {
            status = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(string id)
    {
        var events = await _eventRepository.FindById(id);

        var response = _mapper.Map<EventResponseDto>(events);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }


    [HttpPost("")]
    public async Task<IActionResult> CreateEvent([FromForm] EventCreateRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var image = await ImageUploader(requestDto.CoverImage, "images\\events");

        var draft = new Event
        {
            Title = requestDto.Title,
            Description = requestDto.Description,
            Summary = requestDto.Summary,
            StartsIn = requestDto.StartsIn,
            EndsIn = requestDto.EndsIn,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            CoverImage = new List<CoverImage?>()
            {
                new() { SmallImage = image[0], MediumImage = image[1], LargeImage = image[2], Id = ObjectId.GenerateNewId().ToString()}
            }
        };

        await _eventRepository.CreateAsync(draft);

        var created = await _eventRepository.FindById(draft.ID);

        var response = _mapper.Map<EventResponseDto>(created);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateOne(string id, [FromForm] EventUpdateRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var image = new List<string>();
        if (requestDto.CoverImage != null)
            image = await ImageUploader(requestDto.CoverImage, "images\\events");

        var draft = new Event
        {
            Title = requestDto.Title,
            Description = requestDto.Description,
            Summary = requestDto.Summary,
            StartsIn = requestDto.StartsIn,
            EndsIn = requestDto.EndsIn,
            UpdatedAt = DateTime.Now
        };
        if (image.Count > 0)
            draft.CoverImage = new List<CoverImage?>
            {
                new() { SmallImage = image[0], MediumImage = image[1], LargeImage = image[2] ,Id = ObjectId.GenerateNewId().ToString()}
            };

        var result = await _eventRepository.UpdateOneAsync(id, draft);

        var updated = _mapper.Map<EventResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = updated
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOne(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        await _eventRepository.DeleteOneAsync(id);

        return NoContent();
    }
}