using Microsoft.Extensions.Options;
using OrderSystem.Messaging.Configuration;
using RabbitMQ.Client;

namespace OrderSystem.Messaging.Topology;

internal sealed class RabbitMqTopologyInitializer : IRabbitMqTopologyInitializer
{
	private readonly RabbitMqSettings _settings;

	public RabbitMqTopologyInitializer(IOptions<RabbitMqSettings> options)
	{
		_settings = options.Value;
	}

	public async Task InitializeAsync(IConnection connection, CancellationToken cancellationToken = default)
	{

		await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

		await channel.ExchangeDeclareAsync(
			exchange: _settings.ExchangeName,
			type: ExchangeType.Direct,
			durable: true,
			autoDelete: false,
			arguments: null,
			cancellationToken: cancellationToken);

		await channel.QueueDeclareAsync(
			queue: _settings.QueueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null,
			cancellationToken: cancellationToken);

		await channel.QueueBindAsync(
			queue: _settings.QueueName,
			exchange: _settings.ExchangeName,
			routingKey: _settings.RoutingKey,
			arguments: null,
			cancellationToken: cancellationToken);
	}
}
