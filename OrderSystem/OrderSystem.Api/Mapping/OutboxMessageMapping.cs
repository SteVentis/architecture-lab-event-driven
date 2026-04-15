using Newtonsoft.Json;
using OrderSystem.Contracts.Events;
using OrderSystem.Infrastructure.Models;

namespace OrderSystem.Api.Mapping;

public static class OutboxMessageMapping
{
	public static OutboxMessage ToOutboxMessage(this OrderCreated orderCreated) => new OutboxMessage
	{
		EventId = orderCreated.EventId,
		EventType = nameof(OrderCreated),
		CreatedAt = orderCreated.CreatedAt,
		MessageStatus = OutboxMessageStatus.NotSent,
		Payload = JsonConvert.SerializeObject(orderCreated)
	};
}
