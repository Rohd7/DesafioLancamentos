using Microsoft.Data.Sqlite;
using api_lancamentos.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// (antes do Build)
builder.Services.AddSingleton<IQueuePublisher, QueuePublisher>();
builder.Services.AddSingleton<ITransactionQuery, TransactionQuery>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseCors("AllowAll");
app.Run();