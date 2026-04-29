using Microsoft.Data.Sqlite;
using worker_consolidado.Infrastructure;

namespace worker_consolidado.tests;

public class QueueRepositoryTests
{
    private string CriarBanco()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();

        var createQueue = connection.CreateCommand();
        createQueue.CommandText = @"
        CREATE TABLE Queue (
            Id TEXT PRIMARY KEY,
            Payload TEXT,
            Status TEXT,
            RetryCount INTEGER DEFAULT 0,
            ErrorMessage TEXT,
            CreatedAt TEXT
        );";
        createQueue.ExecuteNonQuery();

        var createDlq = connection.CreateCommand();
        createDlq.CommandText = @"
        CREATE TABLE Queue_DLQ (
            Id TEXT,
            Payload TEXT,
            ErrorMessage TEXT,
            CreatedAt TEXT
        );";
        createDlq.ExecuteNonQuery();

        return path;
    }

    [Fact]
    public async Task Deve_Retornar_Item()
    {
        var dbPath = CriarBanco();

        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();

            var insert = connection.CreateCommand();
            insert.CommandText = @"
            INSERT INTO Queue (Id, Payload, Status, CreatedAt)
            VALUES ($id, 'teste', 'PENDING', $date)";
            insert.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
            insert.Parameters.AddWithValue("$date", DateTime.UtcNow.ToString("o"));
            insert.ExecuteNonQuery();
        }

        var repo = new QueueRepository($"Data Source={dbPath}");

        var item = await repo.GetNextPendingAsync();

        Assert.NotNull(item);
    }

    [Fact]
    public async Task Deve_Enviar_Para_DLQ()
    {
        var dbPath = CriarBanco();
        var id = Guid.NewGuid();

        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();

            var insert = connection.CreateCommand();
            insert.CommandText = @"
            INSERT INTO Queue (Id, Payload, Status, RetryCount, CreatedAt)
            VALUES ($id, 'teste', 'PENDING', 2, $date)";
            insert.Parameters.AddWithValue("$id", id.ToString());
            insert.Parameters.AddWithValue("$date", DateTime.UtcNow.ToString("o"));
            insert.ExecuteNonQuery();
        }

        var repo = new QueueRepository($"Data Source={dbPath}");

        await repo.HandleFailureAsync(id, "teste", "erro");

        using var check = new SqliteConnection($"Data Source={dbPath}");
        check.Open();

        var query = check.CreateCommand();
        query.CommandText = "SELECT COUNT(*) FROM Queue_DLQ";
        var count = Convert.ToInt32(query.ExecuteScalar());

        Assert.Equal(1, count);
    }
}