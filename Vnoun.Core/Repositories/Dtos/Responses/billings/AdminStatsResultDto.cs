namespace Vnoun.Core.Repositories.Dtos.Responses.billings;


public class AdminStatsResultDto
{
    public long Orders { get; set; }
    public long TotalProducts { get; set; }
    public Dictionary<string, decimal> Paid { get; set; }
    public decimal RefundedAmount { get; set; }
    public Dictionary<string, decimal> TodayEarnings { get; set; }
}