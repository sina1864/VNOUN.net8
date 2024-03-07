namespace Vnoun.Application.Requests.Reviews;

public class CreateReviewRequestDto
{
    public string Description { get; set; }
    public decimal Rating { get; set; }
    public string? ProductId { get; set; }
}