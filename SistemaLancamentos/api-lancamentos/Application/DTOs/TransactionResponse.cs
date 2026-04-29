namespace api_lancamentos.Application.DTOs;

public class TransactionResponse
{
    public Guid id { get; set; }
    public string type { get; set; } = default!;
    public decimal amount { get; set; }
    public string currency { get; set; } = default!;
    public string description { get; set; } = default!;
    public string category { get; set; } = default!;
    public DateTime transactionDate { get; set; }
    public DateTime createdAt { get; set; }

    public AccountResponse account { get; set; } = new();
}

public class AccountResponse
{
    public Guid accountId { get; set; }
    public string accountType { get; set; } = default!;
    public Guid ownerId { get; set; }
}