using OrderSystem.Contracts.Events;

namespace OrderSystem.EventProcessor.Handlers;


internal sealed class OrderCreatedEventHandler : IOrderCreatedEventHandler
{
	private readonly ILogger<OrderCreatedEventHandler> _logger;
	public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
	{
		_logger = logger;
	}

	public Task HandleAsync(OrderCreated orderCreatedEvent, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Processing OrderCreatedEvent. EventId: {EventId}, OrderId: {InternalOrderId}", orderCreatedEvent.EventId, orderCreatedEvent.InternalOrderId);

		return Task.CompletedTask;
	}
}
