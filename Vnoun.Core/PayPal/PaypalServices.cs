using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Vnoun.Core.PayPal;
public class PaypalServices
{
    public string BaseUrl = "https://api-m.sandbox.paypal.com";

    private readonly IConfiguration _configuration;
    public PaypalServices(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GenerateAccessToken()
    {
        var clientId = _configuration["PaypalSettings:ClientId"];
        var clientSecret = _configuration["PaypalSettings:ClientSecret"];

        var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

        var form = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        };

        var response = await client.PostAsync($"{BaseUrl}/v1/oauth2/token", new FormUrlEncodedContent(form));

        var responseString = await response.Content.ReadAsStringAsync();

        var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(responseString);

        return responseJson["access_token"].ToString();
    }

    public async Task<HttpResponseMessage> CreateOrder(string currency, decimal amount)
    {
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await GenerateAccessToken());

        var requestBody = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    amount = new
                    {
                        currency_code = currency,
                        value = amount.ToString()
                    }
                }
            }
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        return await client.PostAsync($"{BaseUrl}/v2/checkout/orders", content);
    }

    public async Task<HttpResponseMessage> CaptureOrder(string orderId)
    {
        var payload = new StringContent("{}", Encoding.UTF8, "application/json");

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await GenerateAccessToken());

        return await client.PostAsync($"{BaseUrl}/v2/checkout/orders/{orderId}/capture", payload);
    }
}