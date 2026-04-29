using worker_consolidado.Domain;

namespace worker_consolidado.Infrastructure;

public interface IQueueRepository
{
    Task<QueueItem?> GetNextPendingAsync();
    Task MarkAsProcessingAsync(Guid id);
    Task MarkAsDoneAsync(Guid id);
    Task HandleFailureAsync(Guid id, string payload, string error);
}