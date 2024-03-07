using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Locations;
using Vnoun.Application.Responses.Locations;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;


[ApiController]
[Route("api/v1/location")]
public class LocationController : BaseController<Location>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public LocationController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IUserRepository userRepository, ILocationRepository locationRepository) : base(hostingEnvironment)
    {
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _mapper = mapper;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] string? query)
    {
        var locations = await _locationRepository.GetAllAsync(query);

        var response = _mapper.Map<List<LocationResponseDto>>(locations);

        return Ok(new
        {
            status = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocation(string id)
    {
        var location = await _locationRepository.FindById(id);

        var response = _mapper.Map<LocationResponseDto>(location);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpDelete("")]
    public async Task<IActionResult> DeleteAllLocations()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        await _locationRepository.DeleteAllAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var location = await _locationRepository.FindById(id);
        if (location == null)
            throw new AppException("Location not found", 404);

        await _locationRepository.DeleteOneAsync(id);

        return NoContent();
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        Location location = _mapper.Map<Location>(requestDto);

        await _locationRepository.CreateAsync(location);

        var created = await _locationRepository.FindById(location.ID);

        var response = _mapper.Map<LocationResponseDto>(created);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateLocation(string id, [FromBody] CreateLocationRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var location = await _locationRepository.FindById(id);
        if (location == null)
            throw new AppException("Location not found", 404);

        _mapper.Map(requestDto, location);

        var updated = await _locationRepository.UpdateOneAsync(id, location);

        var response = _mapper.Map<LocationResponseDto>(updated);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }
}