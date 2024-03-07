using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Global;
using Vnoun.Application.Responses.Global;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/global-settings")]
public class GlobalController : BaseController<Global>
{
    private readonly IGlobalRepository _globalRepository;
    private readonly IUserRepository _userRepository;
    public GlobalController(IGlobalRepository globalRepository, IWebHostEnvironment environment, IUserRepository userRepository) : base(environment)
    {
        _globalRepository = globalRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGlobalSettings()
    {
        var globalSettings = await _globalRepository.GetAllAsync();

        return Ok(new
        {
            status = "success",
            results = globalSettings.Count(),
            data = globalSettings
        });
    }

    [HttpPatch]
    public async Task<IActionResult> CreateGlobalSettings([FromForm] GlobalSettingRequestDto createRequestDto)
    {
        if (!ModelState.IsValid)
            throw new AppException("Invalid request", 400);

        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("You are not authorized to perform this action", 401);

        if (createRequestDto.Logo != null)
        {
            if (createRequestDto.Store == null)
            {
                createRequestDto.Store = new GlobalSettingStore()
                {
                    Logo = createRequestDto.Logo
                };
            }
            else
            {
                createRequestDto.Store.Logo = createRequestDto.Logo;
            }
        }

        var global = await _globalRepository.GetGlobalSettings();
        if (global == null)
        {
            global = new()
            {
                Links = new Core.Entities.MetaEntities.Links
                {
                    Facebook = createRequestDto.Links.Facebook ?? "",
                    Gmail = createRequestDto.Links.Gmail ?? "",
                    Twitter = createRequestDto.Links.Twitter ?? "",
                    Github = createRequestDto.Links.Github ?? "",
                    Text = createRequestDto.Links.Text ?? ""
                },
                About = new Core.Entities.MetaEntities.About
                {
                    Heading = createRequestDto.About.Heading ?? "",
                    Summary = createRequestDto.About.Summary ?? "",
                },
                Policy = new Core.Entities.MetaEntities.Policy
                {
                    Description = createRequestDto.Policy.Description ?? ""
                },
                TermsAndConditions = new Core.Entities.MetaEntities.TermsAndConditions
                {
                    Description = createRequestDto.TermsAndConditions.Description ?? ""
                },
                Store = new Core.Entities.MetaEntities.Store
                {
                    Name = createRequestDto.Store.Name ?? ""
                }
            };
        }
        else
        {
            if (createRequestDto.Links != null)
            {
                global.Links = new Core.Entities.MetaEntities.Links
                {
                    Facebook = createRequestDto.Links.Facebook ?? global.Links.Facebook ?? "",
                    Gmail = createRequestDto.Links.Gmail ?? global.Links.Gmail ?? "",
                    Twitter = createRequestDto.Links.Twitter ?? global.Links.Twitter ?? "",
                    Github = createRequestDto.Links.Github ?? global.Links.Github ?? "",
                    Text = createRequestDto.Links.Text ?? global.Links.Text ?? ""
                };
            }

            if (createRequestDto.About != null)
            {
                global.About = new Core.Entities.MetaEntities.About
                {
                    Heading = createRequestDto.About.Heading ?? global.About.Heading ?? "",
                    Summary = createRequestDto.About.Summary ?? global.About.Summary ?? "",
                };
            }

            if (createRequestDto.Policy != null)
            {
                global.Policy = new Core.Entities.MetaEntities.Policy
                {
                    Description = createRequestDto.Policy.Description ?? global.Policy.Description ?? ""
                };
            }

            if (createRequestDto.TermsAndConditions != null)
            {
                global.TermsAndConditions = new Core.Entities.MetaEntities.TermsAndConditions
                {
                    Description = createRequestDto.TermsAndConditions.Description ?? global.TermsAndConditions.Description ?? ""
                };
            }

            if (createRequestDto.Store != null)
            {
                global.Store = new Core.Entities.MetaEntities.Store
                {
                    Name = createRequestDto.Store.Name ?? global.Store.Name ?? ""
                };
            }
        }

        if (createRequestDto.About != null && createRequestDto.About.CoverImage != null)
        {
            var aboutCovetImage = await ImageUploader(createRequestDto.About.CoverImage, "images\\globals");
            global.About.CoverImage = new List<Core.Entities.MetaEntities.Image>(){
                new Core.Entities.MetaEntities.Image(){
                    ID = ObjectId.GenerateNewId().ToString(),
                    SmallImage = aboutCovetImage[0],
                    MediumImage = aboutCovetImage[1],
                    LargeImage = aboutCovetImage[2]
                }
            };
        }

        if (createRequestDto.Store != null && createRequestDto.Store.Logo != null)
        {
            var storeLogo = await ImageUploader(createRequestDto.Store.Logo, "images\\globals");
            global.Store.Logo = new List<Core.Entities.MetaEntities.Image>(){
                new Core.Entities.MetaEntities.Image(){
                    ID = ObjectId.GenerateNewId().ToString(),
                    SmallImage = storeLogo[0],
                    MediumImage = storeLogo[1],
                    LargeImage = storeLogo[2]
                }
            };
        }

        if (global.About == null)
        {
            global.About = new Core.Entities.MetaEntities.About()
            {
                Story = new List<Core.Entities.MetaEntities.Story>(),
                CoverImage = new List<Core.Entities.MetaEntities.Image>(),
                Heading = "",
                Summary = "",
            };
        }

        if (global.About.Story == null)
        {
            global.About.Story = new List<Core.Entities.MetaEntities.Story>();
        }

        if (global.Store == null)
        {
            global.Store = new Core.Entities.MetaEntities.Store()
            {
                Logo = new List<Core.Entities.MetaEntities.Image>(),
                Name = ""
            };
        }

        if (global.Links == null)
        {
            global.Links = new Core.Entities.MetaEntities.Links()
            {
                Facebook = "",
                Gmail = "",
                Github = "",
                Text = "",
                Twitter = ""
            };
        }

        if (global.Policy == null)
        {
            global.Policy = new Core.Entities.MetaEntities.Policy()
            {
                Description = ""
            };
        }

        if (global.TermsAndConditions == null)
        {
            global.TermsAndConditions = new Core.Entities.MetaEntities.TermsAndConditions()
            {
                Description = ""
            };
        }

        await _globalRepository.DeleteAllAsync();
        await _globalRepository.CreateAsync(global);
        var created = await _globalRepository.FindById(global.ID);

        return Ok(new
        {
            status = "success",
            data = created
        });
    }

    [HttpPatch("add-story")]
    public async Task<IActionResult> AddStory([FromForm] AddStoryRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var globals = await _globalRepository.GetAllAsync();
        if (globals.Count == 0)
            throw new AppException("Global settings not found", 404);

        var global = globals[0];
        if (global.About.Story == null)
            global.About.Story = new List<Core.Entities.MetaEntities.Story>();

        var imageUrl = await ImageUploader(requestDto.Photo, "images\\globals");
        global.About.Story.Add(new Core.Entities.MetaEntities.Story
        {
            ID = ObjectId.GenerateNewId().ToString(),
            Text = requestDto.Text,
            Photo = new List<Core.Entities.MetaEntities.Image>(){
                new Core.Entities.MetaEntities.Image(){
                    ID = ObjectId.GenerateNewId().ToString(),
                    SmallImage = imageUrl[0],
                    MediumImage = imageUrl[1],
                    LargeImage = imageUrl[2]
                }
            }
        });

        var updated = await _globalRepository.UpdateOneAsync(global.ID, global);
        return Ok(new
        {
            status = "success",
            data = updated
        });
    }

    [HttpDelete("delete-story/{id}")]
    public async Task<IActionResult> DeleteStory(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var globals = await _globalRepository.GetAllAsync();
        if (globals.Count == 0)
            throw new AppException("Global settings not found", 404);

        var global = globals[0];
        if (global.About.Story == null)
            throw new AppException("Story not found", 404);

        var story = global.About.Story.Find(s => s.ID == id);
        if (story == null)
            throw new AppException("Story not found", 404);

        global.About.Story.Remove(story);

        var updated = await _globalRepository.UpdateOneAsync(global.ID, global);
        if (updated == null)
            throw new AppException("Story not deleted", 400);

        return Ok(new
        {
            status = "success",
            data = updated
        });
    }
}