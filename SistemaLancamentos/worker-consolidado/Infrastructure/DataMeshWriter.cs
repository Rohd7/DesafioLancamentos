using Parquet;
using Parquet.Data;
using Parquet.Schema;
using worker_consolidado.Domain;

namespace worker_consolidado.Infrastructure;

public class DataMeshWriter : IDataMeshWriter
{
    private readonly string _filePath;

    public DataMeshWriter()
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "datamesh");
        Directory.CreateDirectory(dir);

        _filePath = Path.Combine(dir, "transactions.parquet");
    }

    public async Task WriteAsync(Transaction transaction)
{
    var schema = new ParquetSchema(
        new DataField<string>("Id"),
        new DataField<string>("Type"),
        new DataField<decimal>("Amount"),
        new DataField<string>("Currency"),
        new DataField<string>("Description"),
        new DataField<string>("Category"),
        new DataField<string>("TransactionDate"),
        new DataField<string>("CreatedAt"),
        new DataField<string>("AccountId"),
        new DataField<string>("AccountType"),
        new DataField<string>("OwnerId")
    );

    using var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

    using var writer = await ParquetWriter.CreateAsync(schema, stream);

    using (var group = writer.CreateRowGroup())
    {
        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[0], new[] { transaction.Id.ToString() }));
        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[1], new[] { transaction.Type }));
        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[2], new[] { transaction.Amount }));

        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[3], new[] { transaction.Currency ?? "" }));
        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[4], new[] { transaction.Description ?? "" }));
        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[5], new[] { transaction.Category ?? "" }));

        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[6], new[] { transaction.TransactionDate.ToString("o") }));
        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[7], new[] { transaction.CreatedAt.ToString("o") }));

        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[8], new[] { transaction.Account?.AccountId.ToString() ?? "" }));
        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[9], new[] { transaction.Account?.AccountType ?? "" }));
        await group.WriteColumnAsync(new DataColumn((DataField)schema.Fields[10], new[] { transaction.Account?.OwnerId.ToString() ?? "" }));
    }
}
}