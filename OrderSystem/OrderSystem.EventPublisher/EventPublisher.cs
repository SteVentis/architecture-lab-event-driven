using OrderSystem.Infrastructure.Context;
using OrderSystem.Infrastructure.Models;
using OrderSystem.Messaging.Configuration;
using OrderSystem.Messaging.Connection;
using OrderSystem.Messaging.Publisher;

namespace OrderSystem.EventPublisher;

public class EventPublisher : BackgroundService
{
	private readonly ILogger<EventPublisher> _logger;
	private readonly IRabbitMqConnectionProvider _connectionProvider;
	private readonly IRabbitMqPublisher _rabbitMqPublisher;
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public EventPublisher(
		ILogger<EventPublisher> logger,
		IRabbitMqConnectionProvider connectionProvider,
		IRabbitMqPublisher rabbitMqPublisher,
		IServiceScopeFactory serviceScopeFactory)
	{
		_logger = logger;
		_connectionProvider = connectionProvider;
		_rabbitMqPublisher = rabbitMqPublisher;
		_serviceScopeFactory = serviceScopeFactory;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{


		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
			try
			{
				await ProcessOutboxMessageAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in outbox processing loop");
			}

			_logger.LogInformation("Publisher is Ready  ");

			await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
		}

	}

	private async Task ProcessOutboxMessageAsync(CancellationToken stoppingToken)
	{
		var connection = await _connectionProvider.GetConnectionAsync(stoppingToken);

		using var scope = _serviceScopeFactory.CreateScope();

		var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var notSentMessages = dbcontext.OutboxMessages.Where(m => m.MessageStatus == OutboxMessageStatus.NotSent)
			.OrderBy(m => m.CreatedAt)
			.Take(10)
			.ToList();

		if (!notSentMessages.Any())
			return;

		foreach (var message in notSentMessages)
		{

			try
			{
				await _rabbitMqPublisher.PublishAsync(message.Payload, stoppingToken);

				message.MessageStatus = OutboxMessageStatus.Sent;
				message.SentAt = DateTime.UtcNow;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to publish message {MessageId}", message.Id);

				message.MessageStatus = OutboxMessageStatus.NotSent;
			}

		}

		await dbcontext.SaveChangesAsync(stoppingToken);
	}
}
