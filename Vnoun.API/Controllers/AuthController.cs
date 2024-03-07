using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Auth;
using Vnoun.Application.Responses.Auth;
using Vnoun.Application.Responses.User;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Core.Repositories.Dtos.Requests.Auth;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/users")]
public class AuthController : BaseController<User>
{
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    public AuthController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IConfiguration config,
        IUserRepository userRepository) : base(hostingEnvironment)
    {
        _mapper = mapper;
        _config = config;
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    [HttpOptions("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            throw new AppException("The data is not valid", 400);

        var user = await _userRepository.FindUserByEmail(requestDto.Email);
        if (user == null)
            throw new AppException("your email or password is wrong", 401);

        var status = VerifyPassword(requestDto.Password, user.Password);
        if (!status)
            throw new AppException("your email or password is wrong", 401);

        if (user.ID == null)
            throw new AppException("User ID is missing", 500);

        var token = GenerateJsonWebTokenAndAppendToCookies(user);

        var loginResponses = _mapper.Map<UserResponseDto>(user);

        return Ok(new { Status = "success", Token = token, Data = loginResponses });
    }

    [HttpPost("signup")]
    [HttpOptions("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            throw new AppException("The data is not valid", 400);

        var isExist = await _userRepository.FindUserByEmail(requestDto.Email);
        if (isExist != null)
            throw new AppException("the user is exists", 400);

        requestDto.Password = HashPassword(requestDto.Password);

        var draft = _mapper.Map<User>(requestDto);

        await _userRepository.CreateAsync(draft);

        var user = await _userRepository.FindUserByEmail(requestDto.Email);

        var token = GenerateJsonWebTokenAndAppendToCookies(user);

        var responses = _mapper.Map<UserResponseDto>(user);

        return Ok(new { Status = "success", Token = token, Data = responses });
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        ExpireJsonWebTokenAndAppendToCookies();

        return Ok(new { status = "success" });
    }

    [HttpPatch("updateMyPassword")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            throw new AppException("No user exists", 400);

        var user = await _userRepository.FindById(userId);
        if (user == null)
            throw new AppException("No user exists", 404);

        var status = VerifyPassword(requestDto.PasswordCurrent, user.Password);
        if (!status)
            throw new AppException("The password is not correct", 401);

        user.Password = HashPassword(requestDto.Password);
        await _userRepository.UpdateOneAsync(userId, user);

        var token = GenerateJsonWebTokenAndAppendToCookies(user);

        var responses = _mapper.Map<UserResponseDto>(user);

        return Ok(new { Status = "success", Token = token, Data = responses });
    }

    [HttpGet("is-logged-in")]
    public async Task<IActionResult> IsLoggedIn()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { currentUser = false });

        var user = await _userRepository.GetUserByIdWithRelations(userId);
        if (user == null)
            return Ok(new { currentUser = false });

        UserResponseDto userResponse = _mapper.Map<UserResponseDto>(user);

        return Ok(new
        {
            status = "success",
            currentUser = userResponse
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { currentUser = false });

        var user = await _userRepository.FindById(userId);
        if (user == null)
            return Ok(new { currentUser = false });

        UserResponseDto userResponse = _mapper.Map<UserResponseDto>(user);

        return Ok(new
        {
            status = "success",
            data = userResponse
        });
    }

    [HttpPatch("updateMe")]
    public async Task<IActionResult> UpdateMe([FromForm] UpdateMeRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            throw new AppException("The data is not valid", 400);

        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            throw new AppException("No user exists", 400);

        if (requestDto.Password != null)
            throw new AppException("This route is not for password updates. Please use /updateMyPassword.", 400);

        if (requestDto.Photo != null)
            requestDto.Photos = requestDto.Photo;

        List<string>? imageUrl = null;
        if (requestDto.Photos != null && requestDto.Photos.Count > 0)
            imageUrl = await ImageUploader(new FormFileCollection { requestDto.Photos[0] }, "images\\users");

        UpdateUserServiceRequestDto req = new()
        {
            Email = requestDto.Email,
            Name = requestDto.Name,
            Phone = requestDto.Phone,
            Image = imageUrl,
            Password = requestDto.Password,
            PasswordConfirm = requestDto.PasswordConfirm
        };

        var updated = await _userRepository.UpdateUserService(userId, req);
        if (updated == null)
            throw new AppException("The user is not updated", 400);

        UserResponseDto userResponse = _mapper.Map<UserResponseDto>(updated);
        return Ok(new
        {
            status = "success",
            data = new
            {
                user = userResponse
            }
        });
    }

    [HttpDelete("deleteMe")]
    public async Task<IActionResult> DeleteMe()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            throw new AppException("No user exists", 400);

        var result = await _userRepository.DeActivateUser(userId);
        if (!result)
            throw new AppException("The user is not deleted", 400);

        ExpireJsonWebTokenAndAppendToCookies();

        return NoContent();
    }


    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPass([FromBody] ForgotPasswordRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            throw new AppException("The data is not valid", 400);

        var result = await _userRepository.ForgotPassword(requestDto.Email);
        if (!result)
            throw new AppException("The email is not sent", 500);

        return Ok(new
        {
            status = "success",
            messaeg = "The email is sent"
        });
    }

    [HttpPatch("resetPassword/{token}")]
    public async Task<IActionResult> ResetPassword(string token, [FromBody] ResetPasswordRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            throw new AppException("The data is not valid", 400);

        var updated = await _userRepository.ResetPassword(token, requestDto.Password);
        if (updated == null)
            throw new AppException("The password is not reset", 400);

        return Ok(new
        {
            status = "success",
            message = "The password is reset"
        });
    }

    [HttpGet()]
    public async Task<IActionResult> GetUsers([FromQuery] string? query)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { currentUser = false });

        var user = await _userRepository.FindById(userId);
        if (user == null)
            return Ok(new { currentUser = false });

        if (user.Role != "admin")
            return Unauthorized();

        var users = await _userRepository.GetUsers(query);

        List<DetailedUserResponseDto> userResponses = new List<DetailedUserResponseDto>();

        foreach (var user1 in users)
        {
            var mapped = _mapper.Map<DetailedUserResponseDto>(user1);
            userResponses.Add(mapped);
        }

        return Ok(new
        {
            status = "success",
            results = users.Count(),
            data = userResponses,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { currentUser = false });

        var user = await _userRepository.FindById(userId);
        if (user == null)
            return Ok(new { currentUser = false });

        if (user.Role != "admin")
            return Unauthorized();

        var user1 = await _userRepository.FindById(id);
        if (user1 == null)
            throw new AppException("not found", 404);

        return Ok(new
        {
            status = "success",
            data = user1
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { currentUser = false });

        var user = await _userRepository.FindById(userId);
        if (user == null)
            return Ok(new { currentUser = false });

        if (user.Role != "admin")
            return Unauthorized();

        await _userRepository.DeleteOneAsync(id);

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserByAdminRequest requestDto)
    {
        if (!ModelState.IsValid)
            throw new AppException("The data is not valid", 400);

        var userId = GetUserIdFromJsonWebToken();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { currentUser = false });

        var user = await _userRepository.FindById(userId);
        if (user == null)
            return Ok(new { currentUser = false });
        if (user.Role != "admin")
            return Unauthorized();

        var userToUpdate = await _userRepository.FindById(id);

        if (userToUpdate == null)
            throw new AppException("not found", 404);

        if (requestDto.Email != null)
            userToUpdate.Email = requestDto.Email;

        if (requestDto.Name != null)
            userToUpdate.Name = requestDto.Name;

        if (requestDto.Phone != null)
            userToUpdate.Phone = requestDto.Phone;

        if (requestDto.Role != null)
            userToUpdate.Role = requestDto.Role;

        if (requestDto.Active != null)
            userToUpdate.Active = requestDto.Active.Value;

        var updated = await _userRepository.UpdateOneAsync(id, userToUpdate);
        if (updated == null)
            throw new AppException("The user is not updated", 400);

        var userResponse = _mapper.Map<DetailedUserResponseDto>(updated);
        return Ok(new
        {
            status = "success",
            data = userResponse
        });
    }

    private string GenerateJsonWebToken(string userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_config["Jwt:ExpiresInDays"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    private string GenerateJsonWebTokenAndAppendToCookies(User user)
    {
        if (user.ID == null) throw new AppException("User ID is missing", 500);

        var token = GenerateJsonWebToken(user.ID);

        string currentDomain = new Uri(Request.Headers["Origin"]).AbsolutePath;

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["Jwt:ExpiresInDays"])),
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true
        });

        return token;
    }

    private void ExpireJsonWebTokenAndAppendToCookies()
    {
        Response.Cookies.Append("jwt", "loggedout", new CookieOptions
        {
            Expires = DateTime.Now.AddSeconds(5),
            HttpOnly = true
        });
    }
}