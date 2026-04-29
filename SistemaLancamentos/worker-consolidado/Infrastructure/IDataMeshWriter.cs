using worker_consolidado.Domain;

namespace worker_consolidado.Infrastructure;

public interface IDataMeshWriter
{
    Task WriteAsync(Transaction transaction);
}