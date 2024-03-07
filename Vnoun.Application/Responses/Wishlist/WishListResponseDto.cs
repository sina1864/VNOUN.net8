namespace Vnoun.Application.Responses.Wishlist;

public class WishListResponseDto
{
    public string id { get; set; }
    public string _id
    {
        get
        {
            return id;
        }
    }
    public string User { get; set; }
    public Product.ProductResponseDto Product { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string __v { get; set; }
}