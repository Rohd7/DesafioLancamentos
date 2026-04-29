using worker_consolidado.Application.DTOs;
using worker_consolidado.Domain;
using worker_consolidado.Infrastructure;

namespace worker_consolidado.Application;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly IDataMeshWriter _dataMesh;

    public TransactionService(
        ITransactionRepository repository,
        IDataMeshWriter dataMesh)
    {
        _repository = repository;
        _dataMesh = dataMesh;
    }

    public async Task ProcessAsync(TransactionPayload payload)
    {
        var transaction = new Transaction
        {
            Id = payload.transactionId,
            Type = payload.type,
            Amount = payload.amount,

            Currency = payload.currency,
            Description = payload.description,
            Category = payload.category,

            TransactionDate = DateTime.Parse(payload.transactionDate),
            CreatedAt = payload.createdAt,

            Account = new worker_consolidado.Domain.Account
            {
                AccountId = payload.account.accountId,
                AccountType = payload.account.accountType,
                OwnerId = payload.account.ownerId
            }
        };

        await _repository.SaveAsync(transaction);

        await _dataMesh.WriteAsync(transaction);
    }
}