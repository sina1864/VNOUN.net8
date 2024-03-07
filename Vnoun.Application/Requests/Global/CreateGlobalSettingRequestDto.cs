using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Vnoun.Application.Responses.Global;

public class GlobalSettingRequestDto
{
    [JsonPropertyName("links")]
    public GlobalSettingLinks? Links { get; set; }

    [JsonPropertyName("about")]
    public GlobalSettingAbout? About { get; set; }

    [JsonPropertyName("policy")]
    public GlobalSettingDescriptionItem? Policy { get; set; }

    [JsonPropertyName("termsAndConditions")]
    public GlobalSettingDescriptionItem? TermsAndConditions { get; set; }

    [JsonPropertyName("store")]
    public GlobalSettingStore? Store { get; set; }

    [JsonPropertyName("logo")]
    public IFormFileCollection? Logo { get; set; }
}

public class GlobalSettingAbout
{
    [JsonPropertyName("heading")]
    public string? Heading { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("coverImage")]
    public IFormFileCollection? CoverImage { get; set; }
}

public class GlobalSettingLinks
{
    [JsonPropertyName("facebook")]
    public string? Facebook { get; set; }

    [JsonPropertyName("gmail")]
    public string? Gmail { get; set; }

    [JsonPropertyName("twitter")]
    public string? Twitter { get; set; }

    [JsonPropertyName("github")]
    public string? Github { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

public class GlobalSettingDescriptionItem
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class GlobalSettingStore
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("logo")]
    public IFormFileCollection? Logo { get; set; }
}