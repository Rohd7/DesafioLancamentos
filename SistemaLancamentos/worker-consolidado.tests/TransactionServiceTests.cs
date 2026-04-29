using Moq;
using worker_consolidado.Application;
using worker_consolidado.Application.DTOs;
using worker_consolidado.Domain;
using worker_consolidado.Infrastructure;

namespace worker_consolidado.tests;

public class TransactionServiceTests
{
    private TransactionPayload CriarPayloadValido()
    {
        return new TransactionPayload
        {
            transactionId = Guid.NewGuid(),
            externalId = null,
            type = "CREDIT",
            amount = 100,
            currency = "BRL",
            description = "Teste",
            category = "TESTE",
            transactionDate = DateTime.UtcNow.ToString("o"),
            createdAt = DateTime.UtcNow,
            account = new()
            {
                accountId = Guid.NewGuid(),
                accountType = "CASH",
                ownerId = Guid.NewGuid()
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

    [Fact]
    public async Task Deve_Salvar_Transacao()
    {
        var repoMock = new Mock<ITransactionRepository>();
        var dataMeshMock = new Mock<IDataMeshWriter>();

        var service = new TransactionService(repoMock.Object, dataMeshMock.Object);

        var payload = CriarPayloadValido();

        await service.ProcessAsync(payload);

        repoMock.Verify(x => x.SaveAsync(It.IsAny<Transaction>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Enviar_Para_DataMesh()
    {
        var repoMock = new Mock<ITransactionRepository>();
        var dataMeshMock = new Mock<IDataMeshWriter>();

        var service = new TransactionService(repoMock.Object, dataMeshMock.Object);

        var payload = CriarPayloadValido();

        await service.ProcessAsync(payload);

        dataMeshMock.Verify(x => x.WriteAsync(It.IsAny<Transaction>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Mapear_Corretamente()
    {
        var repoMock = new Mock<ITransactionRepository>();
        var dataMeshMock = new Mock<IDataMeshWriter>();

        Transaction? capturada = null;

        repoMock
            .Setup(x => x.SaveAsync(It.IsAny<Transaction>()))
            .Callback<Transaction>(t => capturada = t);

        var service = new TransactionService(repoMock.Object, dataMeshMock.Object);

        var payload = CriarPayloadValido();

        await service.ProcessAsync(payload);

        Assert.NotNull(capturada);
        Assert.Equal(payload.type, capturada!.Type);
        Assert.Equal(payload.amount, capturada.Amount);
    }

    [Fact]
    public async Task Deve_Lancar_Erro_Se_Payload_Nulo()
    {
        var repoMock = new Mock<ITransactionRepository>();
        var dataMeshMock = new Mock<IDataMeshWriter>();

        var service = new TransactionService(repoMock.Object, dataMeshMock.Object);

        await Assert.ThrowsAsync<NullReferenceException>(() =>
            service.ProcessAsync(null!)
        );
    }
}