using worker_consolidado;
using worker_consolidado.Application;
using worker_consolidado.Infrastructure;
using Microsoft.Data.Sqlite;

var builder = Host.CreateApplicationBuilder(args);

// DI
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<IQueueRepository, QueueRepository>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<IDataMeshWriter, DataMeshWriter>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();