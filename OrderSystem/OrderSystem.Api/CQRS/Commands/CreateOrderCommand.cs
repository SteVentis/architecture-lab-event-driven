using MediatR;
using OrderSystem.Api.Common;

namespace OrderSystem.Api.CQRS.Commands;

public record CreateOrderCommand(
	string ProductName,
	int CustomerId,
	decimal TotalAmount) : IRequest<Result<int>>;
