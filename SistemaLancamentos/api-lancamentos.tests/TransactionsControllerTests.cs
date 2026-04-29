using System.Text.Json;
using api_lancamentos.Application.DTOs;
using api_lancamentos.Controllers;
using api_lancamentos.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace api_lancamentos.tests;

public class TransactionsControllerTests
{
    private CreateTransactionRequest CriarRequestValido()
    {
        return new CreateTransactionRequest
        {
            type = "CREDIT",
            amount = 100,
            currency = "BRL",
            description = "Teste",
            category = "TESTE",
            transactionDate = DateTime.UtcNow.ToString("o"),
            account = new()
            {
                accountId = Guid.NewGuid().ToString(),
                accountType = "CASH",
                ownerId = Guid.NewGuid().ToString()
            },
            origin = new()
            {
                channel = "WEB",
                ip = "127.0.0.1",
                userAgent = "teste"
            },
            metadata = new()
            {
                tags = new List<string> { "tag1" },
                notes = "nota"
            }
        };
    }

    private TransactionsController CriarController(
        Mock<IQueuePublisher> publisherMock,
        Mock<ITransactionQuery>? queryMock = null)
    {
        queryMock ??= new Mock<ITransactionQuery>();

        return new TransactionsController(
            publisherMock.Object,
            queryMock.Object
        );
    }

    [Fact]
    public async Task Deve_Publicar_Mensagem_Na_Fila()
    {
        var publisherMock = new Mock<IQueuePublisher>();

        var controller = CriarController(publisherMock);

        var request = CriarRequestValido();

        await controller.Create(request);

        publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Retornar_Ok_Quando_Sucesso()
    {
        var publisherMock = new Mock<IQueuePublisher>();

        var controller = CriarController(publisherMock);

        var request = CriarRequestValido();

        var result = await controller.Create(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Deve_Gerar_Evento_Com_TransactionId()
    {
        var publisherMock = new Mock<IQueuePublisher>();

        string? jsonCapturado = null;

        publisherMock
            .Setup(x => x.PublishAsync(It.IsAny<string>()))
            .Callback<string>(json => jsonCapturado = json);

        var controller = CriarController(publisherMock);

        var request = CriarRequestValido();

        await controller.Create(request);

        Assert.NotNull(jsonCapturado);

        var doc = JsonDocument.Parse(jsonCapturado!);

        var transactionId = doc
            .RootElement
            .GetProperty("payload")
            .GetProperty("transactionId")
            .GetString();

        Assert.False(string.IsNullOrEmpty(transactionId));
    }

    [Fact]
    public async Task Deve_Conter_EventType_Correto()
    {
        var publisherMock = new Mock<IQueuePublisher>();

        string? jsonCapturado = null;

        publisherMock
            .Setup(x => x.PublishAsync(It.IsAny<string>()))
            .Callback<string>(json => jsonCapturado = json);

        var controller = CriarController(publisherMock);

        var request = CriarRequestValido();

        await controller.Create(request);

        var doc = JsonDocument.Parse(jsonCapturado!);

        var eventType = doc.RootElement.GetProperty("eventType").GetString();

        Assert.Equal("TRANSACTION_CREATED", eventType);
    }

    [Fact]
    public async Task Deve_Conter_Payload_Com_Amount_Correto()
    {
        var publisherMock = new Mock<IQueuePublisher>();

        string? jsonCapturado = null;

        publisherMock
            .Setup(x => x.PublishAsync(It.IsAny<string>()))
            .Callback<string>(json => jsonCapturado = json);

        var controller = CriarController(publisherMock);

        var request = CriarRequestValido();

        await controller.Create(request);

        var doc = JsonDocument.Parse(jsonCapturado!);

        var amount = doc
            .RootElement
            .GetProperty("payload")
            .GetProperty("amount")
            .GetDecimal();

        Assert.Equal(request.amount, amount);
    }
}