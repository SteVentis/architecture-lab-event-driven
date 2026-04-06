using OrderSystem.Api.CQRS.Commands;
using OrderSystem.Infrastructure.Models;

namespace OrderSystem.Api.Mapping;

public static class OrderMapping
{
	public static Order MapCreateOrderCommandToOrder(this CreateOrderCommand createOrderCommand) 
		=> new Order
	{
		ProductName = createOrderCommand.ProductName,
		InternalOrderId = Guid.NewGuid().ToString(),
		CreatedAt = DateTime.UtcNow,
		TotalAmount = createOrderCommand.TotalAmount,
		CustomerId = createOrderCommand.CustomerId
	};
}
