using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests;
using Vnoun.Application.Requests.Reviews;
using Vnoun.Application.Responses.Reviews;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/reviews")]
public class ReviewController : BaseController<Review>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public ReviewController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IReviewRepository reviewRepository,
        IUserRepository userRepository, IProductRepository productRepository) : base(hostingEnvironment)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

    [HttpGet("get-my-reviews")]
    public async Task<IActionResult> GetMyReviews()
    {
        var userId = GetUserIdFromJsonWebToken();

        var reviews = await _reviewRepository.GetAllMyReviews(userId);

        var response = _mapper.Map<List<ReviewsResponseDto>>(reviews);

        return Ok(new
        {
            status = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpGet("get-product-reviews/{productId}")]
    public async Task<IActionResult> GetProductreviews(string productId)
    {
        var product = await _productRepository.FindById(productId);
        if (product == null)
            throw new AppException("Product not found", 404);

        var reviews = await _reviewRepository.GetProductReviews(productId);

        var response = _mapper.Map<List<ReviewsResponseDto>>(reviews);

        return Ok(new
        {
            status = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        var userId = GetUserIdFromJsonWebToken();

        if (userId == null)
            throw new AppException("User not found", 404);

        await _reviewRepository.DeleteUsersReview(userId, id);

        return Ok();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateReview(string id, [FromBody] UpdateReviewRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);


        var review = await _reviewRepository.FindById(id);
        if (review == null)
            throw new AppException("Review not found", 404);

        if (review.UserId != userId)
            throw new AppException("Unauthorized", 401);

        review.Description = requestDto.Description ?? review.Description;
        review.Rating = requestDto.Rating ?? review.Rating;

        var updated = await _reviewRepository.UpdateOneAsync(id, review);

        var response = _mapper.Map<ReviewsResponseDto>(updated);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }


    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestDto requestDto, [FromQuery] string product)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var user = await _userRepository.FindById(userId);
        if (user == null)
            throw new AppException("User not found", 404);

        requestDto.ProductId = product;

        var productFound = await _productRepository.FindById(requestDto.ProductId);
        if (productFound == null)
            throw new AppException("Product not found", 404);

        Review newReview = new()
        {
            Description = requestDto.Description,
            Rating = requestDto.Rating,
            UserId = userId,
            ProductId = requestDto.ProductId
        };

        await _reviewRepository.CreateAsync(newReview);

        var created = await _reviewRepository.GetReviewByIdWithUser(newReview.ID);
        var response = _mapper.Map<ReviewsResponseDto>(created);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteReview()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        await _reviewRepository.DeleteAllAsync();

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReviews([FromQuery] string? query)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var reviews = await _reviewRepository.GetAllReviewsWithUser(query);

        var response = _mapper.Map<List<ReviewsResponseDto>>(reviews);

        return Ok(new
        {
            status = "success",
            results = response.Count,
            data = response
        });
    }
}