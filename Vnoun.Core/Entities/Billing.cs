using MongoDB.Bson;
using MongoDB.Entities;
using System.Text.Json.Serialization;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Core.Entities;

[Collection("billings")]
public class Billing : Entity
{
    [Ignore]
    public string _id
    {
        get
        {
            return ID.ToString();
        }
    }

    [Field("orders")]
    public List<Order> Orders { get; set; }

    [Field("paymentId")]
    public string PaymentId { get; set; }

    [Field("user")]
    [JsonIgnore]
    public ObjectId UserId { get; set; }

    [Ignore]
    [JsonPropertyName("user")]
    public User User { get; set; }

    [Field("balance")]
    public decimal Balance { get; set; }

    [Field("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Field("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [Field("chargingStatus")]
    public string ChargingStatus { get; set; }

    [Field("paymentStatus")]
    public string PaymentStatus { get; set; }

    [Field("method")]
    public string Method { get; set; }

    [Field("billingId")]
    public string BillingId { get; set; }

    [Field("brand")]
    public string Brand { get; set; }

    [Field("shipping")]
    public Shipping Shipping { get; set; }

    [Field("email")]
    public string Email { get; set; }

    [Field("last4")]
    public string Last4 { get; set; }

    [Field("currency")]
    public string Currency { get; set; }

    [Field("time")]
    public DateTime Time { get; set; }

    [Field("delivered")]
    public bool Delivered { get; set; }

    [Field("deliveredAt")]
    public DateTime? DeliveredAt { get; set; }

    public string Country { get; set; }

    [Field("__v")]
    [Ignore]
    public int? Version { get; set; }
}