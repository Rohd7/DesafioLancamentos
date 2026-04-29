using System.Text.Json;
using worker_consolidado.Application;
using worker_consolidado.Application.DTOs;
using worker_consolidado.Infrastructure;

namespace worker_consolidado;

public class Worker(
    ILogger<Worker> logger,
    ITransactionService service,
    IQueueRepository queueRepository
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("🔎 Buscando item na fila...");

            var item = await queueRepository.GetNextPendingAsync();

            if (item == null)
            {
                logger.LogInformation("📭 Fila vazia");
                await Task.Delay(2000, stoppingToken);
                continue;
            }

            logger.LogInformation("📥 Item encontrado: {id}", item.Id);

            await queueRepository.MarkAsProcessingAsync(item.Id);

            try
            {
                // 🔥 AGORA DESERIALIZA O EVENTO COMPLETO
                var evento = JsonSerializer.Deserialize<TransactionEvent>(item.Payload);

                if (evento == null)
                    throw new Exception("Evento inválido");

                if (evento.payload == null)
                    throw new Exception("Payload interno inválido");

                // 🔥 USA SOMENTE O PAYLOAD INTERNO
                await service.ProcessAsync(evento.payload);

                await queueRepository.MarkAsDoneAsync(item.Id);

                logger.LogInformation("✅ Processado com sucesso - TransactionId: {id}", evento.payload.transactionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar item, aplicando retry");

                await queueRepository.HandleFailureAsync(
                    item.Id,
                    item.Payload,
                    ex.Message
                );
            }
        }
    }
}