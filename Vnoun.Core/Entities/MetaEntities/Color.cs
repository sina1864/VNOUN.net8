using MongoDB.Entities;
using System.Text.Json.Serialization;

namespace Vnoun.Core.Entities.MetaEntities;

public class Color : Entity
{
    [Ignore]
    [JsonPropertyName("_id")]
    public string _id
    {
        get
        {
            if (ID == null)
            {
                return "";
            }
            return ID.ToString();
        }
    }

    [Field("colorName")]
    public string? ColorName { get; set; }

    [Field("price")]
    public double? Price { get; set; }

    [Field("priceDiscount")]
    public double? PriceDiscount { get; set; }

    [Field("colorCode")]
    public string? ColorCode { get; set; }

    [Field("quantity")]
    public int? Quantity { get; set; }

    [Field("images")]
    public List<Image>? Images { get; set; }

    [Field("sizes")]
    public List<Size>? Sizes { get; set; }
}