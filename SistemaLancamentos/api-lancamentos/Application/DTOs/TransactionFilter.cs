namespace api_lancamentos.Application.DTOs;

public class TransactionFilter
{
    public string? type { get; set; }
    public string? category { get; set; }
    public DateTime? startDate { get; set; }
    public DateTime? endDate { get; set; }
    public decimal? minAmount { get; set; }
    public decimal? maxAmount { get; set; }
    public Guid? accountId { get; set; }
    public string? accountType { get; set; }
}