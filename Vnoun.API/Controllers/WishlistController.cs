using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Wishlist;
using Vnoun.Application.Responses.Product;
using Vnoun.Application.Responses.Wishlist;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/wishlist")]
public class WishlistController : BaseController<Wishlist>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public WishlistController(IMapper mapper, IUserRepository userRepository, IWishlistRepository wishlistRepository, IProductRepository productRepository, IWebHostEnvironment hostingEnvironment) : base(hostingEnvironment)
    {
        _wishlistRepository = wishlistRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWishlist([FromQuery] string? query)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access.", 401);

        var wishlists = await _wishlistRepository.GetWishListsForUser(userId, query);
        List<WishListResponseDto> response = new List<WishListResponseDto>();

        foreach (var wishlist in wishlists)
        {
            var product = await _productRepository.FindById(wishlist.ProductId);
            if (product == null) continue;

            var productResponse = _mapper.Map<ProductResponseDto>(product);
            response.Add(new WishListResponseDto
            {
                id = wishlist.ID,
                User = wishlist.UserId,
                Product = productResponse,
                CreatedAt = wishlist.CreatedAt.ToString(),
                UpdatedAt = wishlist.UpdatedAt.ToString(),
                __v = wishlist.Version
            });
        }

        return Ok(new
        {
            success = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateWishlist([FromForm] WishlistDto requestDto)
    {
        var draft = new Wishlist
        {
            UserId = requestDto.User,
            ProductId = requestDto.Product,
        };

        await _wishlistRepository.CreateAsync(draft);
        await _productRepository.IncreaseWishedCount(requestDto.Product);
        var created = await _wishlistRepository.FindById(draft.ID);

        return Ok(new
        {
            status = "success",
            data = created
        });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllItems()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access.", 401);

        await _wishlistRepository.DeleteAllItemsForUser(userId);

        return NoContent();
    }

    [HttpDelete("delete-items-with-ids")]
    public async Task<IActionResult> DeleteItemsWithIds([FromBody] DeleteWishlistRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access.", 401);

        await _wishlistRepository.DeleteWishListsByIdsForUser(userId, requestDto.ids);

        return NoContent();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteItem(string productId)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access.", 401);

        var admin = _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized access.", 401);

        var product = _productRepository.FindById(productId);
        if (product == null)
            throw new AppException("Product not found.", 404);

        await _wishlistRepository.DeleteAllByProductId(productId, userId);

        return NoContent();
    }
}