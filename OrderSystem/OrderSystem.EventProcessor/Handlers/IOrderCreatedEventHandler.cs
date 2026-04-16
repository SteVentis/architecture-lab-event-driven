using OrderSystem.Contracts.Events;

namespace OrderSystem.EventProcessor.Handlers;

public interface IOrderCreatedEventHandler
{
	Task HandleAsync(OrderCreated orderCreatedEvent, CancellationToken cancellationToken = default);
}
