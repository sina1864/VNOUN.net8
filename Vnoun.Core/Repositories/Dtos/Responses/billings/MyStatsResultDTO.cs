namespace Vnoun.Core.Repositories.Dtos.Responses.billings;


public class MyStatsResultDto
{
    public int Orders { get; set; }
    public Dictionary<string, decimal> Paid { get; set; }
    public decimal RefundedAmount { get; set; }
}