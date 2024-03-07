using MongoDB.Bson;
using MongoDB.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Vnoun.Core.Entities.MetaEntities;

namespace Vnoun.Core.Entities;

[Collection("users")]
public class User : Entity
{
    [Ignore]
    public string _id
    {
        get
        {
            return ID.ToString();
        }
    }

    [Field("name")]
    public string? Name { get; set; }

    [Field("email")]
    public string? Email { get; set; }

    public List<Photo> Photo { get; set; } = new()
    {
        new Photo
        {
            ID =  ObjectId.GenerateNewId().ToString(),
            SmallImage = "default.png",
            MediumImage = "default.png",
            LargeImage = "default.png"
        }
    };

    [Field("role")]
    public string Role { get; set; } = "User";

    [Field("password")]
    [System.Text.Json.Serialization.JsonIgnore]
    public string? Password { get; set; }

    [Field("passwordConfirm")]
    [Ignore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string? PasswordConfirm { get; set; }

    [Field("phone")]
    public string? Phone { get; set; }

    [Field("passwordChangedAt")]
    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime? PasswordChangedAt { get; set; }

    [Field("passwordResetToken")]
    [System.Text.Json.Serialization.JsonIgnore]
    public string? PasswordResetToken { get; set; }

    [Field("passwordResetExpires")]
    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime? PasswordResetExpires { get; set; }

    [Field("active")]
    public bool Active { get; set; } = true;

    [Ignore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<Post>? Posts { get; set; }

    [Ignore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<Card>? Bag { get; set; }

    [Ignore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<Wishlist>? Wishlist { get; set; }

    [Ignore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<Notification>? MyNotifications { get; set; }

    [Field("__v")]
    [JsonProperty("__v")]
    [Ignore]
    public int Version { get; set; }
}

public enum Role
{
    [Display(Name = "User")]
    User,
    [Display(Name = "admin")]
    Admin
}