using api_lancamentos.Application.DTOs;

namespace api_lancamentos.Infrastructure;

public interface ITransactionQuery
{
    Task<List<TransactionResponse>> GetAsync(TransactionFilter filter);
    Task<TransactionResponse?> GetByIdAsync(Guid id);
}