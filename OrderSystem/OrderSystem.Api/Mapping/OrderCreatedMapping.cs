using OrderSystem.Contracts.Events;
using OrderSystem.Infrastructure.Models;

namespace OrderSystem.Api.Mapping;

public static class OrderCreatedMapping
{
	public static OrderCreated ToOrderCreatedEvent(this Order order) => new OrderCreated
	{
		EventId = Guid.NewGuid().ToString(),
		InternalOrderId = order.InternalOrderId,
		CreatedAt = order.CreatedAt,
		CustomerId = order.CustomerId,
		TotalAmount = order.TotalAmount
	};
}
