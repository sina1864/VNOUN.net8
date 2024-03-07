using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Entities;
using Newtonsoft.Json.Linq;
using Stripe;
using System.Text;
using Vnoun.Application.Requests.Billings;
using Vnoun.Core.Entities;
using Vnoun.Core.PayPal;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[ApiController]
[Route("api/v1/billing")]
public class BillingController : BaseController<Billing>
{
    private readonly IBillingRepository _billingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICardRepository _cardRepository;
    private readonly PaypalServices _paypalServices;
    private readonly IConfiguration _configuration;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    public BillingController(IMapper mapper, IConfiguration configuration, PaypalServices paypalServices,
        ICardRepository cardRepository, INotificationRepository notificationRepository, IWebHostEnvironment hostingEnvironment,
        IBillingRepository billingRepository, IUserRepository userRepository)
        : base(hostingEnvironment)
    {
        _billingRepository = billingRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _cardRepository = cardRepository;
        _paypalServices = paypalServices;
        _configuration = configuration;
        _mapper = mapper;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] string? query)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            return Unauthorized();

        var admin = await _userRepository.GetAdminById(userId);
        if (admin == null)
            return Unauthorized();

        var billings = await _billingRepository.GetAllAsync(query);

        foreach (var billing in billings)
            billing.User = await _userRepository.FindById(billing.UserId.ToString());

        return Ok(new
        {
            status = "success",
            results = billings.Count,
            data = billings
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            return Unauthorized();

        var billing = await _billingRepository.FindById(id);

        if (billing == null)
            return NotFound();

        if (billing.UserId.ToString() != userId)
        {
            var admin = await _userRepository.GetAdminById(userId);
            if (admin == null)
            {
                return Unauthorized();
            }
        }

        billing.User = await _userRepository.FindById(billing.UserId.ToString());

        return Ok(new
        {
            success = "success",
            data = billing
        });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userid = GetUserIdFromJsonWebToken();
        if (userid == null)
            return Unauthorized();

        var result = await _billingRepository.GetMyStatsAsync(userid);

        return Ok(new
        {
            success = "success",
            data = result
        });
    }

    [HttpGet("admin-stats")]
    public async Task<IActionResult> GetAdminStats()
    {
        var userid = GetUserIdFromJsonWebToken();
        if (userid == null)
            return Unauthorized();

        var admin = await _userRepository.GetAdminById(userid);
        if (admin == null)
            return Unauthorized();

        var result = await _billingRepository.GetAdminStatsAsync();

        return Ok(new
        {
            success = "success",
            data = result
        });
    }

    [HttpGet("get-my-billings")]
    public async Task<IActionResult> GetMyBillings([FromQuery] string? query)
    {
        var userid = GetUserIdFromJsonWebToken();
        if (userid == null)
            return Unauthorized();

        var result = await _billingRepository.GetMyBillingsAsync(userid, query);

        return Ok(new
        {
            success = "success",
            results = result.Count,
            data = result
        });
    }

    [HttpGet("user-data/{userIdTofind}")]
    public async Task<IActionResult> GetUserData(string userIdTofind, [FromQuery] string? query)
    {
        var userid = GetUserIdFromJsonWebToken();
        if (userid == null)
            return Unauthorized();

        var admin = await _userRepository.GetAdminById(userid);
        if (admin == null)
            return Unauthorized();

        var user = await _userRepository.FindById(userIdTofind);
        if (user == null)
            return NotFound();

        var result = await _billingRepository.GetMyBillingsAsync(userIdTofind, query);

        return Ok(new
        {
            success = "success",
            results = result.Count,
            data = result
        });
    }

    [HttpPatch("deliver-order/{id}")]
    public async Task<IActionResult> DeliverOrder(string id, [FromQuery] string Delivered)
    {
        var userid = GetUserIdFromJsonWebToken();
        if (userid == null)
            return Unauthorized();

        var admin = await _userRepository.GetAdminById(userid);
        if (admin == null)
            return Unauthorized();

        bool delivered = false;
        if (Delivered == "true")
            delivered = true;
        else if (Delivered != "false")
            return BadRequest();

        var billing = await _billingRepository.FindById(id);
        if (billing == null)
            return NotFound();

        billing.Delivered = delivered;
        billing.DeliveredAt = DateTime.Now;

        var updated = await _billingRepository.UpdateOneAsync(id, billing);

        if (updated == null)
        {
            return BadRequest(new
            {
                message = "Failed to update the billing"
            });
        }

        var notif = await _notificationRepository.PushNotification(updated.UserId.ToString(), new Core.Repositories.Dtos.Requests.Notifications.PushNotificationServiceRequestDto
        {
            Title = "Order Delivery",
            Description = delivered
                ? $"Your order [{updated._id}] has been delivered"
                : $"status updated to be undelivered for {updated._id}",
            Link = $"/orders/{updated.ID}"
        });

        return Ok(new
        {
            success = "success",
            data = new
            {
                order = updated
            }
        });
    }

    [HttpPost("checkout/paypal")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutWithPaypalRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.FindById(userId);
        if (user == null)
            return NotFound();

        var cards = await _cardRepository.GetCardsWithIds(requestDto.CardIds);
        var amount = await _cardRepository.GetAmountForCards(cards);
        var order = await _paypalServices.CreateOrder("USD", amount);
        var tojson = await order.Content.ReadAsStringAsync();

        return Content(tojson, "application/json");
    }

    [HttpPost("checkout/paypal/{orderId}/capture")]
    public async Task<IActionResult> CheckoutCapture(string orderId, [FromBody] CheckoutWithPaypalCaptureRequestDto requestDto)
    {
        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.FindById(userId);
        if (user == null)
            return NotFound();

        var captureResult = await _paypalServices.CaptureOrder(orderId);
        if (!captureResult.IsSuccessStatusCode)
            return BadRequest();

        var captureData = await captureResult.Content.ReadAsStringAsync();
        var captureJson = JObject.Parse(captureData);

        var cards = await _cardRepository.GetCardsWithIds(requestDto.CardIds);

        var orders = await _cardRepository.getCartDataAndUpdateQuantityOfProducts(cards, user.ID.ToString());

        if (orders == null || orders.Count == 0)
        {
            return BadRequest(new
            {
                message = "No orders found in the request"
            });
        }

        var billing = new Billing
        {
            UserId = ObjectId.Parse(user.ID),
            Orders = orders,
            Balance = (decimal)captureJson["purchase_units"][0]["payments"]["captures"][0]["amount"]["value"] * 100,
            Currency = (string)captureJson["purchase_units"][0]["payments"]["captures"][0]["amount"]["currency_code"],
            PaymentStatus = (string)captureJson["status"],
            Delivered = false,
            CreatedAt = DateTime.Now,
            Brand = "PayPal",
            Email = requestDto.Shipping.Email,
            PaymentId = captureJson["id"].ToString(),
            Shipping = new Core.Entities.MetaEntities.Shipping
            {
                Name = captureJson["purchase_units"][0]["shipping"]["name"]["full_name"].ToString(),
                Address = new Core.Entities.MetaEntities.Address
                {
                    City = captureJson["purchase_units"][0]["shipping"]["address"]["admin_area_2"].ToString(),
                    Line1 = captureJson["purchase_units"][0]["shipping"]["address"]["address_line_1"].ToString(),
                    PostalCode = captureJson["purchase_units"][0]["shipping"]["address"]["postal_code"].ToString(),
                    Country = captureJson["purchase_units"][0]["shipping"]["address"]["country_code"].ToString(),
                    ID = ObjectId.GenerateNewId().ToString(),
                },
                ID = ObjectId.GenerateNewId().ToString(),
                Phone = requestDto.Shipping.Phone,
            },
            Country = (string)captureJson["purchase_units"][0]["shipping"]["address"]["country_code"],
            Method = "paypal",
            BillingId = captureJson["id"].ToString(),
            Time = DateTime.Now,
            ChargingStatus = (string)captureJson["status"],
            UpdatedAt = DateTime.Now,
        };

        await billing.SaveAsync();
        await _cardRepository.DeleteWithIds(requestDto.CardIds);

        var notif = await _notificationRepository.PushNotification(user.ID.ToString(), new Core.Repositories.Dtos.Requests.Notifications.PushNotificationServiceRequestDto
        {
            Description = $"Your payment {captureJson["status"]}",
            Link = "/orders/" + billing.ID,
            Title = "payment information"
        });

        return Content(captureData, "application/json");
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> Stripe()
    {
        string req = Encoding.UTF8.GetString(HttpContext.Request.BodyReader.ReadAsync().Result.Buffer);
        var reqObj = JObject.Parse(req);
        var requestDto = new StripePaymentRequestDto();

        requestDto.CardIds = reqObj["cardIds"].ToObject<List<string>>();
        requestDto.City = reqObj["city"].ToString();
        requestDto.Country = reqObj["country"].ToString();
        requestDto.Line1 = reqObj["line1"].ToString();
        requestDto.Zip = reqObj["zip"].ToString();
        requestDto.Email = reqObj["email"].ToString();
        requestDto.Name = reqObj["name"].ToString();
        requestDto.Phone = reqObj["phone"].ToString();

        var userId = GetUserIdFromJsonWebToken();
        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.FindById(userId);
        if (user == null)
            return NotFound();

        var cards = await _cardRepository.GetCardsWithIds(requestDto.CardIds);
        var amount = await _cardRepository.GetAmountForCards(cards);

        var metadata = new Dictionary<string, string>();

        for (int i = 0; i < cards.Count; i++)
        {
            metadata.Add("order_" + i, cards[i].ID.ToString());
        }

        var random = new Random();
        metadata.Add("user", user.ID.ToString());
        metadata.Add("billingId", random.Next(0, 10000).ToString() + DateTime.Now.ToString());

        var secretKey = _configuration["Stripe:SecretKey"];
        StripeConfiguration.ApiKey = secretKey;

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions()
            {
                Enabled = true
            },
            Metadata = metadata,
            Shipping = new ChargeShippingOptions
            {
                Address = new AddressOptions
                {
                    City = requestDto.City,
                    Country = requestDto.Country,
                    Line1 = requestDto.Line1,
                    PostalCode = requestDto.Zip,
                },
                Name = requestDto.Name,
                Phone = requestDto.Phone
            },
            ReceiptEmail = requestDto.Email,
        };

        var stripeservice = new PaymentIntentService();

        var paymentIntent = stripeservice.Create(options);

        return Ok(new
        {
            status = "success",
            data = paymentIntent.ClientSecret
        });
    }
}