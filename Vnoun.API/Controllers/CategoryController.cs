using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Category;
using Vnoun.Application.Responses.Category;
using Vnoun.Core.Entities;
using Vnoun.Core.Entities.MetaEntities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/category")]
public class CategoryController : BaseController<Category>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public CategoryController(IMapper mapper, IWebHostEnvironment hostingEnvironment,
        ICategoryRepository categoryRepository, IUserRepository userRepository) : base(hostingEnvironment)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(string id)
    {
        var result = await _categoryRepository.FindById(id);
        if (result == null)
            throw new AppException("No document found with that ID", 404);

        var response = _mapper.Map<CategoryResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories([FromQuery] string? query)
    {
        var results = await _categoryRepository.GetAllAsync(query);

        var response = _mapper.Map<List<CategoryResponseDto>>(results);

        return Ok(new
        {
            status = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        if (!ModelState.IsValid)
            throw new AppException("Invalid Data", 400);

        var image = await ImageUploader(requestDto.Photo, "images\\categories");

        if (image.Count <= 2)
            return Ok(new { ss = image.Count });

        var category = new Category
        {
            Title = requestDto.Title,
            Order = requestDto.Order,
            Heading = requestDto.Heading,
            SubHeading = requestDto.SubHeading,
            Photo = new List<Photo>
            {
                new() { SmallImage = image[0], MediumImage = image[1], LargeImage = image[2] ,ID = ObjectId.GenerateNewId().ToString()}
            }
        };

        await _categoryRepository.CreateAsync(category);

        var response = _mapper.Map<CategoryResponseDto>(category);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCategory(string id, [FromForm] CategoryUpdateRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        if (!ModelState.IsValid)
            throw new AppException("Invalid Data", 400);

        var image = new List<string>();
        if (requestDto.Photo != null)
            image = await ImageUploader(requestDto.Photo, "images/categories");

        var category = new Category
        {
            Title = requestDto.Title,
            Order = requestDto.Order,
            Heading = requestDto.Heading,
            SubHeading = requestDto.SubHeading,
            Photo = new List<Photo>()
        };
        if (image.Count > 0)
            category.Photo = new List<Photo>
            {
                new() { SmallImage = image[0], MediumImage = image[1], LargeImage = image[2] ,ID = ObjectId.GenerateNewId().ToString()}
            };

        var result = await _categoryRepository.UpdateOneAsync(id, category);

        var response = _mapper.Map<CategoryResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        await _categoryRepository.DeleteOneAsync(id);
        return Ok(new
        {
            status = "success",
        });
    }

    [HttpPatch("add-filter-secion/{id}")]
    public async Task<IActionResult> AddFilterSection(string id, [FromBody] FilterDataCreateRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        if (!ModelState.IsValid)
            throw new AppException("Invalid Data", 400);

        var category = await _categoryRepository.FindById(id);

        if (category == null)
            throw new AppException("No category with that id found", 404);

        if (category.FilterData == null)
            category.FilterData = new List<FilterData>();

        category.FilterData.Add(new FilterData
        {
            ID = ObjectId.GenerateNewId().ToString(),
            PropertyName = requestDto.PropertyName,
            Values = requestDto.Values,
            SelectionStyle = requestDto.SelectionStyle,
            DefaultValue = requestDto.DefaultValue,
            Order = requestDto.Order
        });

        var result = await _categoryRepository.UpdateOneAsync(id, category);
        var response = _mapper.Map<CategoryResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPatch("edit-filter-section/{categoryId}/{filterId}")]
    public async Task<IActionResult> EditFilterSection(string categoryId, string filterId, [FromBody] FilterDataUpdateRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        if (!ModelState.IsValid)
            throw new AppException("Invalid Data", 400);

        var category = await _categoryRepository.FindById(categoryId);

        if (category.FilterData == null)
            throw new AppException("Category has no filter to edit", 404);

        var oldFilterData = category.FilterData.Find(x => x.ID == filterId);
        if (oldFilterData != null)
        {
            oldFilterData.PropertyName = requestDto.PropertyName ?? oldFilterData.PropertyName;
            oldFilterData.Values = requestDto.Values ?? oldFilterData.Values;
            oldFilterData.SelectionStyle = requestDto.SelectionStyle ?? oldFilterData.SelectionStyle;
            oldFilterData.DefaultValue = requestDto.DefaultValue ?? oldFilterData.DefaultValue;
            oldFilterData.Order = requestDto.Order ?? oldFilterData.Order;
        }

        var result = await _categoryRepository.UpdateOneAsync(categoryId, category);

        return Ok(new
        {
            status = "success",
            data = result
        });
    }

    [HttpPatch("delete-filter-section/{categoryId}/{filterId}")]
    public async Task<IActionResult> DeleteFilterSection(string categoryId, string filterId)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var category = await _categoryRepository.FindById(categoryId);

        if (category.FilterData == null)
            throw new AppException("No category with that id found", 404);

        await _categoryRepository.DeleteFilterSectionAsync(categoryId, filterId);

        var result = await _categoryRepository.FindById(categoryId);

        var response = _mapper.Map<CategoryResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == string.Empty)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        await _categoryRepository.DeleteAllCategories();

        return Ok(new
        {
            status = "success",
        });
    }
}