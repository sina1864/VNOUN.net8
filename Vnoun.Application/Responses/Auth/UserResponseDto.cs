using Vnoun.Application.Responses.Cards;
using Vnoun.Application.Responses.MetaResponses;
using Vnoun.Application.Responses.Wishlist;

namespace Vnoun.Application.Responses.Auth;

public class UserResponseDto : BaseResponse
{
    public string Name { get; set; }
    public string Email { get; set; }
    public List<PhotoResponseDto> Photo { get; set; }
    public string Role { get; set; }
    public string Phone { get; set; }
    public List<CardsResponseDto>? Bag { get; set; }
    public List<WishListResponseDto>? Wishlist { get; set; }
}