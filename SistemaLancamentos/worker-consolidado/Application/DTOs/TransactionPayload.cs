namespace worker_consolidado.Application.DTOs;

public class TransactionEvent
{
    public Guid eventId { get; set; }
    public string eventType { get; set; } = default!;
    public string eventVersion { get; set; } = default!;
    public DateTime eventTimestamp { get; set; }
    public string source { get; set; } = default!;
    public Guid correlationId { get; set; }
    public Guid traceId { get; set; }
    public TransactionPayload payload { get; set; } = new();
}

public class TransactionPayload
{
    public Guid transactionId { get; set; }
    public string? externalId { get; set; }
    public string type { get; set; } = default!;
    public decimal amount { get; set; }
    public string currency { get; set; } = default!;
    public string description { get; set; } = default!;
    public string category { get; set; } = default!;
    public string transactionDate { get; set; } = default!;
    public DateTime createdAt { get; set; }

    public Account account { get; set; } = new();
    public Origin origin { get; set; } = new();
    public Metadata metadata { get; set; } = new();
}

public class Account
{
    public Guid accountId { get; set; }
    public string accountType { get; set; } = default!;
    public Guid ownerId { get; set; }
}

public class Origin
{
    public string channel { get; set; } = default!;
    public string ip { get; set; } = default!;
    public string userAgent { get; set; } = default!;
}

public class Metadata
{
    public List<string> tags { get; set; } = new();
    public string notes { get; set; } = default!;
}