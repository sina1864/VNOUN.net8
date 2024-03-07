using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Vnoun.Application.Requests.Category;

public class CategoryCreateRequestDto
{
    [Required(ErrorMessage = "Category must have a title 😠")]
    [JsonProperty("title")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Category must have an order 😠")]
    [JsonProperty("order")]
    public int Order { get; set; }

    [Required(ErrorMessage = "Category must have a heading 😠")]
    [JsonProperty("heading")]
    public string Heading { get; set; }

    [Required(ErrorMessage = "Category must have a subHeading 😠")]
    [JsonProperty("subHeading")]
    public string SubHeading { get; set; }

    [Required(ErrorMessage = "Event Must Has a Photo")]
    [JsonProperty("photo")]
    public IFormFileCollection Photo { get; set; }
}