using Microsoft.Extensions.Options;
using OrderSystem.Messaging.Configuration;
using RabbitMQ.Client;

namespace OrderSystem.Messaging.Connection;

internal sealed class RabbitMqConnectionProvider : IRabbitMqConnectionProvider
{
	private readonly RabbitMqSettings _settings;
	private readonly SemaphoreSlim _lock = new(1, 1);
	private IConnection? _connection;

	public RabbitMqConnectionProvider(IOptions<RabbitMqSettings> settings)
	{
		_settings = settings.Value;
	}

	public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
	{
		if (_connection is { IsOpen: true })
			return _connection;


		await _lock.WaitAsync(cancellationToken);

		try
		{
			if (_connection is { IsOpen: true })
				return _connection;

			var factory = new ConnectionFactory
			{
				HostName = _settings.HostName,
				Port = _settings.Port,
				UserName = _settings.UserName,
				Password = _settings.Password,
				VirtualHost = _settings.VirtualHost,
				ClientProvidedName = _settings.ClientProvidedName,

				AutomaticRecoveryEnabled = true,
				NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
			};

			_connection =  await factory.CreateConnectionAsync(cancellationToken);

			return _connection;
		}
		finally
		{
			_lock.Release();
		}
	}

	public async ValueTask DisposeAsync() 
	{ 
		if (_connection is not null)
		{
			await _connection.CloseAsync();
			await _connection.DisposeAsync();
		}

		_lock.Dispose();
	}
}