using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vnoun.API.Exceptions;
using Vnoun.Application.Requests.Cards;
using Vnoun.Application.Responses.Cards;
using Vnoun.Application.Responses.Product;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/bag")]
public class CardController : BaseController<Card>
{
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    public CardController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IProductRepository productRepository, IUserRepository userRepository, ICardRepository cardRepository) : base(hostingEnvironment)
    {
        _cardRepository = cardRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpGet("get-the-top-products")]
    public async Task<IActionResult> GetTopProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await _productRepository.GetMostAddedToCardService(page, pageSize);

        var response = _mapper.Map<List<ProductResponseDto>>(products);

        return Ok(new
        {
            success = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllMyCards()
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access", 401);

        await _cardRepository.DeleteCardsForUser(userId);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCards([FromQuery] string? query)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access", 401);

        var cards = await _cardRepository.GetCardsForUser(userId);

        var response = _mapper.Map<List<CardsResponseDto>>(cards);

        return Ok(new
        {
            success = "success",
            results = response.Count,
            data = response
        });
    }

    [HttpDelete("delete-by-ids")]
    public async Task<IActionResult> DeleteCardsWithIds([FromBody] CardDeleteIdsRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access", 401);

        await _cardRepository.DeleteCardsWithIds(requestDto.CardIds, userId);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCard(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access", 401);

        var card = await _cardRepository.FindById(id);
        if (card == null)
            throw new AppException("Card not found", 404);

        if (card.UserId != userId)
            throw new AppException("Unauthorized access", 401);

        await _cardRepository.DeleteCardsWithIds(new List<string> { id }, userId);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCard(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access", 401);

        var card = await _cardRepository.FindById(id);
        if (card == null)
            throw new AppException("Card not found", 404);

        if (card.UserId != userId)
            throw new AppException("Unauthorized access", 401);

        var response = _mapper.Map<CardsResponseDto>(card);

        return Ok(new
        {
            success = "success",
            data = response
        });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCard(string id, [FromBody] UpdateCardRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            throw new AppException("Unauthorized access", 401);

        var card = await _cardRepository.FindById(id);
        if (card == null)
            throw new AppException("Card not found", 404);

        if (card.UserId != userId)
            throw new AppException("Unauthorized access", 401);

        card.Color = requestDto.Color ?? card.Color;
        card.NumberOfOrders = requestDto.NumberOfOrders ?? card.NumberOfOrders;
        card.Size = requestDto.Size ?? card.Size;

        var updated = await _cardRepository.UpdateOneAsync(id, card);
        var response = _mapper.Map<CardsResponseDto>(updated);

        return Ok(new
        {
            success = "success",
            data = response
        });
    }
}