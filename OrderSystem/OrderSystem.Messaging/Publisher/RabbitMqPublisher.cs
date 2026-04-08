using Microsoft.Extensions.Options;
using OrderSystem.Messaging.Configuration;
using OrderSystem.Messaging.Connection;
using RabbitMQ.Client;
using System.Text;

namespace OrderSystem.Messaging.Publisher;

internal sealed class RabbitMqPublisher : IRabbitMqPublisher
{
	private readonly IRabbitMqConnectionProvider _connectionProvider;
	private readonly RabbitMqSettings _rabbitMqSettings;

	public RabbitMqPublisher(
		IRabbitMqConnectionProvider connectionProvider, 
		IOptions<RabbitMqSettings> rabbitMqSettings)
	{
		_connectionProvider = connectionProvider;
		_rabbitMqSettings = rabbitMqSettings.Value;
	}

	public async Task PublishAsync(string message, CancellationToken cancellationToken = default)
	{
		var connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

		await using var channel = await connection.CreateChannelAsync();

		var body = Encoding.UTF8.GetBytes(message);

		await channel.BasicPublishAsync(exchange: _rabbitMqSettings.ExchangeName,
										routingKey: _rabbitMqSettings.RoutingKey,
										mandatory: false,
										body: body,
										cancellationToken: cancellationToken);
	}	
}