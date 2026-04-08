using Microsoft.Extensions.Hosting;
using OrderSystem.Messaging.Connection;
using OrderSystem.Messaging.Topology;

namespace OrderSystem.Messaging;

internal class RabbitMqStartupInitializer : IHostedService
{
	private readonly IRabbitMqConnectionProvider _connectionProvider;
	private readonly IRabbitMqTopologyInitializer _topologyInitializer;

	public RabbitMqStartupInitializer(
		IRabbitMqConnectionProvider connectionProvider, 
		IRabbitMqTopologyInitializer topologyInitializer)
	{
		_connectionProvider = connectionProvider;
		_topologyInitializer = topologyInitializer;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

		await _topologyInitializer.InitializeAsync(connection, cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
