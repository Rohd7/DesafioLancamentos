namespace worker_consolidado.Domain;

public class QueueItem
{
    public Guid Id { get; set; }
    public string Payload { get; set; } = default!;
    public string Status { get; set; } = "PENDING"; // PENDING, PROCESSING, DONE
    public DateTime CreatedAt { get; set; }
}