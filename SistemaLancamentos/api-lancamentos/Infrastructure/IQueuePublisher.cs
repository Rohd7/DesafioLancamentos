namespace api_lancamentos.Infrastructure;

public interface IQueuePublisher
{
    Task PublishAsync(string payload);
}