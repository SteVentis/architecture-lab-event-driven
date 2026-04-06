using MediatR;
using OrderSystem.Api.Common;
using OrderSystem.Api.CQRS.Commands;
using OrderSystem.Api.Mapping;
using OrderSystem.Api.Services;

namespace OrderSystem.Api.CQRS.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<int>>
{
	private readonly IOrderService _orderService;
	public CreateOrderHandler(IOrderService orderService)
	{
		_orderService = orderService;
	}

	public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
	{
		var order = request.MapCreateOrderCommandToOrder();

		var result = await _orderService.AddOrder(order, cancellationToken);

		return result;
	}
}
