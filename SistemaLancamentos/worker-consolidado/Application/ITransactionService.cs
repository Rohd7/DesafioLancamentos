using worker_consolidado.Application.DTOs;

namespace worker_consolidado.Application;

public interface ITransactionService
{
    Task ProcessAsync(TransactionPayload payload);
}