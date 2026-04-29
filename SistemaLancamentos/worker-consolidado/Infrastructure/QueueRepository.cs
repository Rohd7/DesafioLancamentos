using Microsoft.Data.Sqlite;
using worker_consolidado.Domain;

namespace worker_consolidado.Infrastructure;

public class QueueRepository : IQueueRepository
{
    private readonly string _connectionString;

    public QueueRepository(string? connectionString = null)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            var dbPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "database.db"
            );

            _connectionString = $"Data Source={dbPath}";
        }
        else
        {
            _connectionString = connectionString;
        }
    }

    public async Task<QueueItem?> GetNextPendingAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Payload, Status, CreatedAt
            FROM Queue
            WHERE Status = 'PENDING'
            ORDER BY CreatedAt
            LIMIT 1;
        ";

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new QueueItem
            {
                Id = Guid.TryParse(reader.GetString(0), out var id) ? id : Guid.Empty,
                Payload = reader.GetString(1),
                Status = reader.GetString(2),
                CreatedAt = DateTime.TryParse(reader.GetString(3), out var dt)
                    ? dt
                    : DateTime.MinValue
            };
        }

        return null;
    }

    public async Task MarkAsProcessingAsync(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Queue SET Status = 'PROCESSING' WHERE Id = $id;
        ";
        command.Parameters.AddWithValue("$id", id.ToString());

        await command.ExecuteNonQueryAsync();
    }

    public async Task MarkAsDoneAsync(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Queue SET Status = 'DONE' WHERE Id = $id;
        ";
        command.Parameters.AddWithValue("$id", id.ToString());

        await command.ExecuteNonQueryAsync();
    }

    public async Task HandleFailureAsync(Guid id, string payload, string error)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var select = connection.CreateCommand();
        select.CommandText = "SELECT RetryCount FROM Queue WHERE Id = $id";
        select.Parameters.AddWithValue("$id", id.ToString());

        var retryObj = await select.ExecuteScalarAsync();
        var retryCount = retryObj != null ? Convert.ToInt32(retryObj) : 0;

        retryCount++;

        if (retryCount >= 3)
        {
            var insertDlq = connection.CreateCommand();
            insertDlq.CommandText = @"
                INSERT INTO Queue_DLQ (Id, Payload, ErrorMessage, CreatedAt)
                VALUES ($id, $payload, $error, $createdAt);
            ";

            insertDlq.Parameters.AddWithValue("$id", id.ToString());
            insertDlq.Parameters.AddWithValue("$payload", payload);
            insertDlq.Parameters.AddWithValue("$error", error);
            insertDlq.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("o"));

            await insertDlq.ExecuteNonQueryAsync();

            var delete = connection.CreateCommand();
            delete.CommandText = "DELETE FROM Queue WHERE Id = $id";
            delete.Parameters.AddWithValue("$id", id.ToString());

            await delete.ExecuteNonQueryAsync();
        }
        else
        {
            var update = connection.CreateCommand();
            update.CommandText = @"
                UPDATE Queue
                SET RetryCount = $retry,
                    Status = 'PENDING',
                    ErrorMessage = $error
                WHERE Id = $id;
            ";

            update.Parameters.AddWithValue("$retry", retryCount);
            update.Parameters.AddWithValue("$error", error);
            update.Parameters.AddWithValue("$id", id.ToString());

            await update.ExecuteNonQueryAsync();
        }
    }
}