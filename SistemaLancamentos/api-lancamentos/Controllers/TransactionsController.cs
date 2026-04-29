using System.Text.Json;
using api_lancamentos.Application.DTOs;
using api_lancamentos.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace api_lancamentos.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IQueuePublisher _publisher;
    private readonly ITransactionQuery _query;

    public TransactionsController(IQueuePublisher publisher, ITransactionQuery query)
    {   
        _publisher = publisher;
        _query = query;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionRequest request)
    {
        var now = DateTime.UtcNow;

        var evento = new
        {
            eventId = Guid.NewGuid(),
            eventType = "TRANSACTION_CREATED",
            eventVersion = "1.0",
            eventTimestamp = now,
            source = "lancamentos-api",
            correlationId = Guid.NewGuid(),
            traceId = Guid.NewGuid(),
            payload = new
            {
                transactionId = Guid.NewGuid(),
                externalId = (string?)null,
                type = request.type,
                amount = request.amount,
                currency = request.currency,
                description = request.description,
                category = request.category,
                transactionDate = request.transactionDate,
                createdAt = now,
                account = request.account,
                origin = request.origin,
                metadata = request.metadata
            }
        };

        var json = JsonSerializer.Serialize(evento);

        await _publisher.PublishAsync(json);

        return Ok(evento);
    }
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] TransactionFilter filter)
    {
        var result = await _query.GetAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _query.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}