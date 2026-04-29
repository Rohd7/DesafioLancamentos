namespace api_lancamentos.Application.DTOs;

public class CreateTransactionRequest
{
    public string type { get; set; } = default!;
    public decimal amount { get; set; }
    public string currency { get; set; } = "BRL";
    public string description { get; set; } = default!;
    public string category { get; set; } = default!;
    public string transactionDate { get; set; } = default!;

    public AccountRequest account { get; set; } = new();
    public OriginRequest origin { get; set; } = new();
    public MetadataRequest metadata { get; set; } = new();
}

public class AccountRequest
{
    public string accountId { get; set; } = default!;
    public string accountType { get; set; } = default!;
    public string ownerId { get; set; } = default!;
}

public class OriginRequest
{
    public string channel { get; set; } = default!;
    public string ip { get; set; } = default!;
    public string userAgent { get; set; } = default!;
}

public class MetadataRequest
{
    public List<string> tags { get; set; } = new();
    public string notes { get; set; } = default!;
}