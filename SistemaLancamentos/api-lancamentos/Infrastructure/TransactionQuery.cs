using Microsoft.Data.Sqlite;
using api_lancamentos.Application.DTOs;

namespace api_lancamentos.Infrastructure;

public class TransactionQuery : ITransactionQuery
{
    private readonly string _connectionString;

public TransactionQuery()
{
    var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "database.db");
    _connectionString = $"Data Source={dbPath}";
}
    public async Task<List<TransactionResponse>> GetAsync(TransactionFilter filter)
    {
        var result = new List<TransactionResponse>();

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        var query = @"
        SELECT 
            Id,
            Type,
            Amount,
            Currency,
            Description,
            Category,
            TransactionDate,
            CreatedAt,
            AccountId,
            AccountType,
            OwnerId
        FROM Transactions
        WHERE 1=1";

        var cmd = conn.CreateCommand();

        if (!string.IsNullOrEmpty(filter.type))
        {
            query += " AND Type = $type";
            cmd.Parameters.AddWithValue("$type", filter.type);
        }

        if (!string.IsNullOrEmpty(filter.category))
        {
            query += " AND Category = $category";
            cmd.Parameters.AddWithValue("$category", filter.category);
        }

        if (filter.minAmount != null)
        {
            query += " AND Amount >= $minAmount";
            cmd.Parameters.AddWithValue("$minAmount", filter.minAmount);
        }

        if (filter.maxAmount != null)
        {
            query += " AND Amount <= $maxAmount";
            cmd.Parameters.AddWithValue("$maxAmount", filter.maxAmount);
        }

        if (filter.startDate != null)
        {
            query += " AND TransactionDate >= $startDate";
            cmd.Parameters.AddWithValue("$startDate", filter.startDate.Value.ToString("o"));
        }

        if (filter.endDate != null)
        {
            query += " AND TransactionDate <= $endDate";
            cmd.Parameters.AddWithValue("$endDate", filter.endDate.Value.ToString("o"));
        }

        if (filter.accountId != null)
        {
            query += " AND AccountId = $accountId";
            cmd.Parameters.AddWithValue("$accountId", filter.accountId.ToString());
        }

        // 🔥 NOVO FILTRO
        if (!string.IsNullOrEmpty(filter.accountType))
        {
            query += " AND AccountType = $accountType";
            cmd.Parameters.AddWithValue("$accountType", filter.accountType);
        }

        cmd.CommandText = query;

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new TransactionResponse
            {
                id = Guid.Parse(reader["Id"]?.ToString() ?? Guid.Empty.ToString()),
                type = reader["Type"]?.ToString() ?? "",
                amount = Convert.ToDecimal(reader["Amount"]),

                currency = reader["Currency"]?.ToString() ?? "",
                description = reader["Description"]?.ToString() ?? "",
                category = reader["Category"]?.ToString() ?? "",

                transactionDate = DateTime.TryParse(reader["TransactionDate"]?.ToString(), out var td)
                    ? td
                    : DateTime.MinValue,

                createdAt = DateTime.TryParse(reader["CreatedAt"]?.ToString(), out var ca)
                    ? ca
                    : DateTime.MinValue,

                account = new AccountResponse
                {
                    accountId = Guid.TryParse(reader["AccountId"]?.ToString(), out var accId)
                        ? accId
                        : Guid.Empty,

                    accountType = reader["AccountType"]?.ToString() ?? "",

                    ownerId = Guid.TryParse(reader["OwnerId"]?.ToString(), out var ownerId)
                        ? ownerId
                        : Guid.Empty
                }
            });
        }

        return result;
    }

    public async Task<TransactionResponse?> GetByIdAsync(Guid id)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        SELECT 
            Id,
            Type,
            Amount,
            Currency,
            Description,
            Category,
            TransactionDate,
            CreatedAt,
            AccountId,
            AccountType,
            OwnerId
        FROM Transactions 
        WHERE Id = $id";

        cmd.Parameters.AddWithValue("$id", id.ToString());

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new TransactionResponse
        {
            id = Guid.Parse(reader["Id"]?.ToString() ?? Guid.Empty.ToString()),
            type = reader["Type"]?.ToString() ?? "",
            amount = Convert.ToDecimal(reader["Amount"]),

            currency = reader["Currency"]?.ToString() ?? "",
            description = reader["Description"]?.ToString() ?? "",
            category = reader["Category"]?.ToString() ?? "",

            transactionDate = DateTime.TryParse(reader["TransactionDate"]?.ToString(), out var td)
                ? td
                : DateTime.MinValue,

            createdAt = DateTime.TryParse(reader["CreatedAt"]?.ToString(), out var ca)
                ? ca
                : DateTime.MinValue,

            account = new AccountResponse
            {
                accountId = Guid.TryParse(reader["AccountId"]?.ToString(), out var accId)
                    ? accId
                    : Guid.Empty,

                accountType = reader["AccountType"]?.ToString() ?? "",

                ownerId = Guid.TryParse(reader["OwnerId"]?.ToString(), out var ownerId)
                    ? ownerId
                    : Guid.Empty
            }
        };
    }
}