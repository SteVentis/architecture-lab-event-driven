using RabbitMQ.Client;

namespace OrderSystem.Messaging.Connection;

public interface IRabbitMqConnectionProvider : IAsyncDisposable 
{
	Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
}
