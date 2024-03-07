using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Cards;
using Vnoun.Application.Requests.Product;
using Vnoun.Application.Responses.Product;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Color = Vnoun.Core.Entities.MetaEntities.Color;
using Image = Vnoun.Core.Entities.MetaEntities.Image;
using Order = MongoDB.Entities.Order;
using Size = Vnoun.Core.Entities.MetaEntities.Size;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/product")]
public class ProductController : BaseController<Post>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWishlistRepository _wishlistRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IMapper _mapper;
    public ProductController(IMapper mapper, ICardRepository cardRepository, IWishlistRepository wishlistRepository, IWebHostEnvironment hostingEnvironment, IProductRepository productRepository, IUserRepository userRepository) : base(
        hostingEnvironment)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _wishlistRepository = wishlistRepository;
        _cardRepository = cardRepository;
    }

    [HttpGet("get-the-most-added-to-cart-products")]
    public async Task<IActionResult> GetMostAddedToCart()
    {
        var result = await _productRepository.GetMostAddedToCardService(1, 10);

        var response = _mapper.Map<List<ProductResponseDto>>(result);

        return Ok(new
        {
            status = "success",
            results = result.Count,
            data = response
        });
    }

    [HttpGet("get-the-most-wishlisted-products")]
    public async Task<IActionResult> GetMostWishListedProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _productRepository.GetMostWishListedProductsService(page, pageSize);

        var response = _mapper.Map<List<ProductResponseDto>>(result);

        return Ok(new
        {
            status = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpGet("get-trendy-products")]
    public async Task<IActionResult> GetTrendyProducts()
    {
        var result = await _productRepository.GetProductsByFilter(query => query
            .Sort(a => a.NumberOfViewers, Order.Descending)
            .Sort(a => a.RatingsAverage, Order.Descending));

        var response = _mapper.Map<List<ProductResponseDto>>(result);

        return Ok(new
        {
            status = "success",
            results = result.Count,
            data = response
        });
    }

    [HttpGet("get-recommended-products")]
    public async Task<IActionResult> GetRecommendedProducts()
    {
        var result = await _productRepository.GetProductsByFilter(query => query
            .Sort(a => a.Colors[0].Price, Order.Ascending)
            .Sort(a => a.RatingsAverage, Order.Descending)
            .Sort(a => a.CreatedAt, Order.Descending));

        return Ok(new
        {
            status = "success",
            results = result.Count,
            data = result
        });
    }

    [HttpGet("get-newly-added-products")]
    public async Task<IActionResult> GetNewlyAddedProducts()
    {
        var result = await _productRepository.GetProductsByFilter(query => query
            .Sort(a => a.CreatedAt, Order.Descending));

        var response = _mapper.Map<List<ProductResponseDto>>(result);

        return Ok(new
        {
            status = "success",
            results = result.Count,
            data = response
        });
    }

    [HttpGet("get-related-to-current-product")]
    public async Task<IActionResult> GetRelatedToCurrentProduct([FromQuery] string productId)
    {
        var product = await _productRepository.FindById(productId);
        var Category = product.Category;
        var result = await _productRepository.GetProductsByFilter(query => query
            .Match(a => a.ID != productId)
            .Match(a => a.Category == Category)
            .Sort(a => a.NumberOfViewers, Order.Descending)
            .Sort(a => a.RatingsAverage, Order.Descending)
            .Limit(7));

        var response = _mapper.Map<List<ProductResponseDto>>(result);

        return Ok(new
        {
            status = "success",
            results = result.Count,
            data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var product = new Product
        {
            Category = requestDto.Category,
            CollectionSeason = requestDto.CollectionSeason,
            Description = requestDto.Description,
            Gender = requestDto.Gender,
            GlobalDiscount = requestDto.GlobalDiscount ?? 0.0,
            Name = requestDto.Name,
            Type = requestDto.Type,
            AgeGroup = requestDto.AgeGroup,
            NumberOfViewers = 0,
            RatingsAverage = 4.5,
            RatingsQuantity = 0,
            Tags = requestDto.Tags.Select(x => x.ToLower()).ToList(),
            Version = requestDto.Version
        };

        await _productRepository.CreateAsync(product);

        var response = _mapper.Map<ProductResponseDto>(product);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductCreateRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var product = await _productRepository.FindById(id);
        if (product == null)
            throw new AppException("There is no product with that id", 404);

        product.Category = requestDto.Category ?? product.Category;
        product.CollectionSeason = requestDto.CollectionSeason ?? product.CollectionSeason;
        product.Description = requestDto.Description ?? product.Description;
        product.AgeGroup = requestDto.AgeGroup ?? product.AgeGroup;
        product.Gender = requestDto.Gender ?? product.Gender;
        product.GlobalDiscount = requestDto.GlobalDiscount ?? product.GlobalDiscount;
        product.Name = requestDto.Name ?? product.Name;
        product.Type = requestDto.Type ?? product.Type;
        product.Tags = requestDto.Tags != null ? requestDto.Tags.Select(x => x.ToLower()).ToList() : product.Tags;

        var result = await _productRepository.UpdateOneAsync(id, product);

        var response = _mapper.Map<ProductResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPatch("add-color/{id}")]
    public async Task<IActionResult> AddColor(string id, [FromForm] ColorRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        if (requestDto.Images == null || requestDto.Images.Count == 0)
            throw new AppException("Please provide images for color", 400);

        var product = await _productRepository.FindById(id);
        if (product == null)
            throw new AppException("There is no product with that id", 404);

        var image = await ImageUploader(requestDto.Images, "images\\products");

        if (product.Colors == null)
            product.Colors = new List<Color>();

        product.Colors.Add(new Color
        {
            ID = ObjectId.GenerateNewId().ToString(),
            ColorName = requestDto.ColorName,
            Price = requestDto.Price,
            PriceDiscount = requestDto.PriceDiscount,
            ColorCode = requestDto.ColorCode,
            Quantity = requestDto.Quantity,
            Images = new List<Image>
            { new() {
                SmallImage = image[0],
                MediumImage = image[1],
                LargeImage = image[2] ,
                ID=ObjectId.GenerateNewId().ToString()
                }
            },
            Sizes = requestDto.Sizes != null ? requestDto.Sizes.Select(x => new Size { SizeName = x.Size, Quantity = x.Quantity, _id = ObjectId.GenerateNewId().ToString() }).ToList() : new List<Size>()
        });

        var result = await _productRepository.UpdateOneAsync(id, product);

        var response = _mapper.Map<ProductResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPatch("update-color/{id}")]
    public async Task<IActionResult> UpdateColor(string id, [FromForm] UpdateColorRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var image = new List<string>();
        if (requestDto.Images != null && requestDto.Images.Count > 0)
            image = await ImageUploader(requestDto.Images, "images\\products");

        var product = await _productRepository.FindById(id);

        var oldColor = product.Colors.Find(x => x.ID == requestDto._id);
        if (oldColor != null)
        {
            oldColor.ColorName = requestDto.ColorName ?? oldColor.ColorName;
            oldColor.Price = requestDto.Price ?? oldColor.Price;
            oldColor.PriceDiscount = requestDto.PriceDiscount ?? oldColor.PriceDiscount;
            oldColor.ColorCode = requestDto.ColorCode ?? oldColor.ColorCode;
            oldColor.Quantity = requestDto.Quantity ?? oldColor.Quantity;
            oldColor.Images = image.Count != 0 ? new List<Image>
                { new() { SmallImage = image[0], MediumImage = image[1], LargeImage = image[2] ,ID=ObjectId.GenerateNewId().ToString()} } : oldColor.Images;
        }

        var result = await _productRepository.UpdateOneAsync(id, product);

        var response = _mapper.Map<ProductResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpDelete("delete-color/{id}/{colorName}")]
    public async Task<IActionResult> DeleteColor(string id, string colorName)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var product = await _productRepository.FindById(id);
        if (product == null)
            throw new AppException("There is no product with that id", 404);

        await _productRepository.DeleteColorAsync(id, colorName);

        var result = await _productRepository.FindById(id);
        var response = _mapper.Map<ProductResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] string? query)
    {
        var results = await _productRepository.GetAllAsync(query);

        var response = _mapper.Map<List<ProductResponseDto>>(results);

        return Ok(new
        {
            status = "success",
            results = response.Count,
            data = response,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOneProduct(string id)
    {
        var result = await _productRepository.FindById(id);

        var response = _mapper.Map<ProductResponseDto>(result);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPost("search-for-products")]
    public async Task<IActionResult> SearchForProductsByName([FromBody] ProductSearchRequestDto requestDto)
    {
        var result = await _productRepository.GetProductsByFilter(query => query
            .Match(a => a.Name.ToLower().Contains(requestDto.Text.ToLower()))
            .Match(a => a.Category.ToLower().Contains(requestDto.Text.ToLower()))
            .Sort(a => a.NumberOfViewers, Order.Descending)
            .Sort(a => a.RatingsAverage, Order.Descending));

        var response = _mapper.Map<List<ProductResponseDto>>(result);

        return Ok(new
        {
            status = "success",
            results = result.Count,
            data = response
        });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllProducts()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        await _productRepository.DeleteAllAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOneProduct(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        await _productRepository.DeleteOneAsync(id);

        return NoContent();
    }

    [HttpDelete("discount/{id}")]
    public async Task<IActionResult> DeleteDiscount(string id, [FromBody] RemoveDiscountsRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var product = await _productRepository.FindById(id);
        if (product == null)
            throw new AppException("there is no product with that id", 404);

        product.GlobalDiscount = 0;

        var result = await _productRepository.UpdateOneAsync(id, product);
        if (result == null)
            throw new AppException("discount not removed", 400);

        if (product.Colors != null)
            product.Colors = new List<Color>();

        List<Color> colors = new();

        foreach (var color in product.Colors)
        {
            if (requestDto.ColorIds.Contains(color.ID))
            {
                color.PriceDiscount = 0;
            }
            colors.Add(color);
        }

        result.Colors = colors;

        var updated = await _productRepository.UpdateOneAsync(id, result);

        var response = _mapper.Map<ProductResponseDto>(updated);

        return Ok(new
        {
            status = "success",
            data = response
        });
    }

    [HttpPost("discount/{id}")]
    public async Task<IActionResult> AddDiscount(string id, AddDiscountRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        var product = await _productRepository.FindById(id);
        if (product == null)
            throw new AppException("There is no product with that id", 404);

        var colorIds = requestDto.Discounts.Keys.ToList();

        if (product.Colors != null)
            product.Colors = new List<Color>();

        var colors = product.Colors;
        foreach (var color in colors)
        {
            if (colorIds.Contains(color.ID))
            {
                color.PriceDiscount = requestDto.Discounts[color.ID];
            }
        }

        product.Colors = colors;

        var updated = await _productRepository.UpdateOneAsync(id, product);

        var response = _mapper.Map<ProductResponseDto>(updated);

        return Ok(new
        {
            status = "success",
            product = response
        });
    }

    [HttpDelete("discount/add-global-discount/")]
    public async Task<IActionResult> DeleteGlobalDiscount(RemoveGlobalDiscountRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        List<ProductResponseDto> response = new();
        foreach (var id in requestDto.Ids)
        {
            var product = await _productRepository.FindById(id);
            if (product == null)
                throw new AppException("There is no product with that id", 404);

            product.GlobalDiscount = 0;
            var result = await _productRepository.UpdateOneAsync(id, product);
            if (result == null)
                throw new AppException("Discount not removed", 400);

            response.Add(_mapper.Map<ProductResponseDto>(result));
        }

        return StatusCode(201, new
        {
            status = "success",
            products = response
        });
    }

    [HttpPost("discount/add-global-discount")]
    public async Task<IActionResult> AddGlobalDiscount(AddGlobalDiscountRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized", 401);

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            throw new AppException("Unauthorized", 401);

        List<ProductResponseDto> response = new();
        foreach (var id in requestDto.Ids)
        {
            var product = await _productRepository.FindById(id);
            if (product == null)
                throw new AppException("there is no product with that id", 404);

            product.GlobalDiscount = requestDto.Discount;
            var result = await _productRepository.UpdateOneAsync(id, product);
            if (result == null)
                throw new AppException("discount not added", 400);

            response.Add(_mapper.Map<ProductResponseDto>(result));
        }

        return Ok(new
        {
            status = "success",
            products = response
        });
    }

    [HttpPost("{productId}/wishlist")]
    public async Task<IActionResult> AddWishList(string productId)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access", 401);

        var product = await _productRepository.FindById(productId);
        if (product == null)
            throw new AppException("Product not found", 404);

        var wishList = new Wishlist
        {
            ProductId = productId,
            UserId = userId,
        };

        await _wishlistRepository.CreateAsync(wishList);
        await _productRepository.IncreaseWishedCount(productId);
        var created = await _wishlistRepository.FindById(wishList.ID);

        return Ok(new
        {
            status = "success",
            data = created
        });
    }

    [HttpPost("{productId}/bag")]
    public async Task<IActionResult> CreateCard(string productId, [FromBody] CreateCardRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access", 401);

        var product = await _productRepository.FindById(productId);
        if (product == null)
            throw new AppException("Product not found", 404);

        var card = new Card
        {
            UserId = userId,
            ProductId = productId,
            Color = requestDto.Color,
            Size = requestDto.Size,
            NumberOfOrders = requestDto.NumberOfOrders
        };

        await _cardRepository.CreateAsync(card);
        await _productRepository.IncreaseAddedToCardCount(productId);
        var created = await _cardRepository.FindById(card.ID);

        return Ok(new
        {
            status = "success",
            data = created
        });
    }
}