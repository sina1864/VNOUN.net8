using Microsoft.AspNetCore.Http;

namespace Vnoun.Application.Requests.Product;

public class ColorRequestDto
{
    public string? ColorName { get; set; }
    public double? Price { get; set; }
    public double? PriceDiscount { get; set; }
    public string? ColorCode { get; set; }
    public int? Quantity { get; set; }
    public IFormFileCollection? Images { get; set; }
    public List<SizeRequestDto>? Sizes { get; set; }
}

public class UpdateColorRequestDto : ColorRequestDto
{
    public string _id { get; set; }
}

public class SizeRequestDto
{
    public string Size { get; set; }
    public int Quantity { get; set; }
}