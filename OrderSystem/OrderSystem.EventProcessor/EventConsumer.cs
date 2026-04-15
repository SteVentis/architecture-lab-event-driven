using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderSystem.Contracts.Events;
using OrderSystem.EventProcessor.Handlers;
using OrderSystem.Infrastructure.Context;
using OrderSystem.Infrastructure.Models;
using OrderSystem.Messaging.Configuration;
using OrderSystem.Messaging.Connection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OrderSystem.EventProcessor;

public class EventConsumer : BackgroundService
{
	private readonly ILogger<EventConsumer> _logger;
	private readonly IRabbitMqConnectionProvider _connectionProvider;
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly RabbitMqSettings _rabbitMqSettings;

	private IConnection? _connection;
	private IChannel? _channel;

	public EventConsumer(
		ILogger<EventConsumer> logger,
		IRabbitMqConnectionProvider connectionProvider,
		IServiceScopeFactory serviceScopeFactory,
		IOptions<RabbitMqSettings> rabbitMqSettings)
	{
		_logger = logger;
		_connectionProvider = connectionProvider;
		_serviceScopeFactory = serviceScopeFactory;
		_rabbitMqSettings = rabbitMqSettings.Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_connection = await _connectionProvider.GetConnectionAsync(stoppingToken);
		_channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

		await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

		var consumer = new AsyncEventingBasicConsumer(_channel);

		consumer.ReceivedAsync += async (_, eventArgs) =>
		{
			try
			{
				var body = eventArgs.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);

				await ProcessMessageAsync(message, stoppingToken);

				await _channel.BasicAckAsync(
					deliveryTag: eventArgs.DeliveryTag,
					multiple: false,
					cancellationToken: stoppingToken);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while processing RabbitMQ message");

				await _channel.BasicAckAsync(
					deliveryTag: eventArgs.DeliveryTag,
					multiple: false,
					cancellationToken: stoppingToken);
			}
		};

		await _channel.BasicConsumeAsync(
			queue: _rabbitMqSettings.QueueName,
			autoAck: false,
			consumer: consumer,
			cancellationToken: stoppingToken);

		_logger.LogInformation("Consumer is listening on queue {QueueName}", _rabbitMqSettings.QueueName);

		await Task.Delay(Timeout.Infinite, stoppingToken);
	}

	private async Task ProcessMessageAsync(string message, CancellationToken stoppingToken)
	{
		var orderCreatedEvent = JsonConvert.DeserializeObject<OrderCreated>(message);

		if (orderCreatedEvent is null)
		{
			throw new InvalidOperationException("Message could not be deserialized to OrderCreatedEvent.");
		}

		using var scope = _serviceScopeFactory.CreateScope();

		var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var handler = scope.ServiceProvider.GetRequiredService<IOrderCreatedEventHandler>();


		await using var transaction = await dbcontext.Database.BeginTransactionAsync(stoppingToken);

		var existingMessage = await dbcontext.InboxMessages.FindAsync(orderCreatedEvent.EventId);

		if (existingMessage is not null && existingMessage.ProcessedAt.HasValue)
		{
			_logger.LogInformation("Message with EventId {EventId} has already been processed. Skipping.", orderCreatedEvent.EventId);

			await transaction.CommitAsync(stoppingToken);
			return;
		}

		if (existingMessage is null)
		{
			var inboxMessage = new InboxMessage
			{
				EventId = orderCreatedEvent.EventId,
				EventType = nameof(OrderCreated),
				ReceivedAt = DateTime.UtcNow,
				MessageStatus = InboxMessageStatus.Received
			};

			await dbcontext.InboxMessages.AddAsync(inboxMessage);
			await dbcontext.SaveChangesAsync(stoppingToken);

			existingMessage = inboxMessage;
		}

		await handler.HandleAsync(orderCreatedEvent, stoppingToken);

		existingMessage.ProcessedAt = DateTime.UtcNow;
		existingMessage.MessageStatus = InboxMessageStatus.Processed;

		await dbcontext.SaveChangesAsync(stoppingToken);
		await transaction.CommitAsync(stoppingToken);

	}


	public override async Task StopAsync(CancellationToken stoppingToken)
	{
		if(_channel is not null)
		{
			await _channel.CloseAsync(stoppingToken);
			await _channel.DisposeAsync();
		}

		await base.StopAsync(stoppingToken);
	}
}