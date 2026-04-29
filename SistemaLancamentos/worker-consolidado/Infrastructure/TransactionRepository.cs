using Microsoft.Data.Sqlite;
using worker_consolidado.Domain;

namespace worker_consolidado.Infrastructure;

public class TransactionRepository : ITransactionRepository
{
    private readonly string _connectionString;

    public TransactionRepository()
    {
        var dbPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "database.db"
        );

        _connectionString = $"Data Source={dbPath}";
    }

    public async Task SaveAsync(Transaction transaction)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO Transactions (
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
        )
        VALUES (
            $id,
            $type,
            $amount,
            $currency,
            $description,
            $category,
            $transactionDate,
            $createdAt,
            $accountId,
            $accountType,
            $ownerId
        );
        ";

        command.Parameters.AddWithValue("$id", transaction.Id.ToString());
        command.Parameters.AddWithValue("$type", transaction.Type);
        command.Parameters.AddWithValue("$amount", transaction.Amount);

        command.Parameters.AddWithValue("$currency", transaction.Currency ?? "");
        command.Parameters.AddWithValue("$description", transaction.Description ?? "");
        command.Parameters.AddWithValue("$category", transaction.Category ?? "");

        command.Parameters.AddWithValue("$transactionDate", transaction.TransactionDate.ToString("o"));
        command.Parameters.AddWithValue("$createdAt", transaction.CreatedAt.ToString("o"));

        // 🔥 conta segura
        command.Parameters.AddWithValue(
            "$accountId",
            transaction.Account?.AccountId != null
                ? transaction.Account.AccountId.ToString()
                : DBNull.Value
        );

        command.Parameters.AddWithValue(
            "$accountType",
            transaction.Account?.AccountType ?? ""
        );

        command.Parameters.AddWithValue(
            "$ownerId",
            transaction.Account?.OwnerId != null
                ? transaction.Account.OwnerId.ToString()
                : DBNull.Value
        );

        await command.ExecuteNonQueryAsync();
    }
}