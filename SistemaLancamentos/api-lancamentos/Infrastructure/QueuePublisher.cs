using Microsoft.Data.Sqlite;

namespace api_lancamentos.Infrastructure;

public class QueuePublisher : IQueuePublisher
{
    private readonly string _connectionString;

    public QueuePublisher()
    {
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "database.db");
        _connectionString = $"Data Source={dbPath}";
    }

    public async Task PublishAsync(string payload)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO Queue (Id, Payload, Status, CreatedAt)
        VALUES ($id, $payload, 'PENDING', $createdAt);
        ";

        cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("$payload", payload);
        cmd.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("o"));

        await cmd.ExecuteNonQueryAsync();
    }
}