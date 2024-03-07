using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Post;
using Vnoun.Application.Responses.Post;
using Vnoun.Core.Entities;
using Vnoun.Core.Entities.MetaEntities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/posts")]
public class PostController : BaseController<Post>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    public PostController(IMapper mapper, IPostRepository postRepository, IUserRepository userRepository, IWebHostEnvironment hostEnvironment) : base(hostEnvironment)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] string? query)
    {
        var posts = await _postRepository.GetAllPostsWithPublisher(query);

        var response = _mapper.Map<List<PostResponseDto>>(posts);

        return Ok(new
        {
            status = "success",
            results = response.Count(),
            data = response
        });
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePost([FromForm] PostCreateRequestDto createRequestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("You are not authorized to perform this action", 401);

        createRequestDto.PublisherId = userId;
        var draft = _mapper.Map<Post>(createRequestDto);

        if (createRequestDto.CoverImage != null)
        {
            var imageUrl = await ImageUploader(createRequestDto.CoverImage, "images\\posts");
            draft.CoverImage = new List<CoverImage>(){
                new CoverImage(){
                    Id = ObjectId.GenerateNewId().ToString(),
                    SmallImage = imageUrl[0],
                    MediumImage = imageUrl[1],
                    LargeImage = imageUrl[2]
                }
            };
        }

        if (createRequestDto.Images != null)
        {
            var imageList = new List<PostImage>();
            foreach (var image in createRequestDto.Images)
            {
                IFormFileCollection coll = new FormFileCollection() { image };
                var imageUrls = await ImageUploader(coll, "images\\posts");
                imageList.Add(new PostImage()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    SmallImage = imageUrls[0],
                    MediumImage = imageUrls[1],
                    LargeImage = imageUrls[2]
                });
            }
            draft.Images = imageList;
        }

        draft.PublisherId = ObjectId.Parse(createRequestDto.PublisherId);
        draft.CreatedAt = DateTime.Now;

        await _postRepository.CreateAsync(draft);

        var added = await _postRepository.FindById(draft.ID);

        added.Publisher = await _userRepository.FindById(added.PublisherId.ToString());
        if (added.UpdatedAt == null)
        {
            added.UpdatedAt = added.CreatedAt;
        }

        var response = _mapper.Map<PostResponseDto>(added);

        return StatusCode(201, new
        {
            status = "success",
            data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var post = await _postRepository.GetPostWithPublisher(id);
        if (post == null)
            throw new AppException("Post not found", 404);

        var response = _mapper.Map<PostResponseDto>(post);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePost(string id, [FromForm] PostUpdateRequestDto createRequestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var post = await _postRepository.FindById(id);
        if (post == null)
            throw new AppException("Post not found", 404);

        if (createRequestDto.CoverImage != null)
        {
            var imageUrl = await ImageUploader(createRequestDto.CoverImage, "images\\posts");
            post.CoverImage = new List<CoverImage>(){
                new CoverImage(){
                    Id = ObjectId.GenerateNewId().ToString(),
                    SmallImage = imageUrl[0],
                    MediumImage = imageUrl[1],
                    LargeImage = imageUrl[2]
                }
            };
        }

        if (createRequestDto.Images != null)
        {
            var imageList = new List<PostImage>();
            foreach (var image in createRequestDto.Images)
            {
                IFormFileCollection coll = new FormFileCollection() { image };
                var imageUrls = await ImageUploader(coll, "images\\posts");
                imageList.Add(new PostImage()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    SmallImage = imageUrls[0],
                    MediumImage = imageUrls[1],
                    LargeImage = imageUrls[2]
                });
            }
            post.Images = imageList;
        }

        post.Title = createRequestDto.Title ?? post.Title;
        post.Description = createRequestDto.Description ?? post.Description;
        post.Summary = createRequestDto.Summary ?? post.Summary;
        post.Pinned = createRequestDto.Pinned ?? post.Pinned;
        post.UpdatedAt = DateTime.Now;

        var updated = await _postRepository.UpdateOneAsync(id, post);
        if (updated == null)
            throw new AppException("Post not updated", 400);

        var response = _mapper.Map<PostResponseDto>(updated);
        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("You are not authorized to perform this action", 401);

        var post = await _postRepository.FindById(id);
        if (post == null)
            throw new AppException("Post not found", 404);

        await _postRepository.DeleteOneAsync(id);

        return NoContent();
    }
}