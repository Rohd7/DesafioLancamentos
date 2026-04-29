namespace worker_consolidado.Domain;

public class Transaction
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public decimal Amount { get; set; }

    public string Currency { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Category { get; set; } = default!;

    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public Account Account { get; set; } = new();
}

public class Account
{
    public Guid AccountId { get; set; }
    public string AccountType { get; set; } = default!;
    public Guid OwnerId { get; set; }
}