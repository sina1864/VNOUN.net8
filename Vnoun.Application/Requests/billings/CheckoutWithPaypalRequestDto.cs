namespace Vnoun.Application.Requests.Billings;


public class CheckoutWithPaypalRequestDto
{
    public List<string> CardIds { get; set; }
}

public class CheckoutWithPaypalCaptureRequestDto
{
    public List<string> CardIds { get; set; }
    public CheckoutShippingDto Shipping { get; set; }

}

public class CheckoutShippingDto
{
    public string Phone { get; set; }
    public string Email { get; set; }
}