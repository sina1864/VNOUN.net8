using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Vnoun.API.Controllers;

public class BaseController<T> : ControllerBase where T : class
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    protected BaseController(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    protected async Task<List<string>> ImageUploader(IFormFileCollection files, string name)
    {
        try
        {
            if (OperatingSystem.IsLinux())
            {
                name = name.Replace("\\", "/");
            }

            var targetFolder = Path.Combine(_hostingEnvironment.WebRootPath, name);

            if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

            var imageUrl = await ImageUploaderHelper.UploadAndResizeImages(files, "", targetFolder);

            return imageUrl;
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }

    protected string? GetUserIdFromJsonWebToken()
    {
        var token = string.Empty;
        if (Request.Headers.ContainsKey("Authorization") && Request.Headers["Authorization"].ToString().StartsWith("Bearer"))
            token = Request.Headers["Authorization"].ToString()[7..];
        else if
            (Request.Cookies.ContainsKey("jwt")) token = Request.Cookies["jwt"];

        if (token == string.Empty) return token;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM");
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "sub").Value;
            return userId;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    protected string JsonNullRemove(object obj)
    {
        var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        return json;
    }
}