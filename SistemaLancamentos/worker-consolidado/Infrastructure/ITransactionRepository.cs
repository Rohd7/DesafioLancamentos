using worker_consolidado.Domain;

namespace worker_consolidado.Infrastructure;

public interface ITransactionRepository
{
    Task SaveAsync(Transaction transaction);
}