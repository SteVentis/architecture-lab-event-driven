using RabbitMQ.Client;

namespace OrderSystem.Messaging.Topology;

public interface IRabbitMqTopologyInitializer
{
	Task InitializeAsync(IConnection connection, CancellationToken cancellationToken = default);
}
