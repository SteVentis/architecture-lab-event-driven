using MediatR;
using OrderSystem.Api.Common;
using OrderSystem.Api.CQRS.Commands;
using OrderSystem.Api.Mapping;
using OrderSystem.Api.Services;

namespace OrderSystem.Api.CQRS.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<int>>
{
	private readonly IOrderService _orderService;
	private readonly ILogger<CreateOrderHandler> _logger;
	public CreateOrderHandler(IOrderService orderService, 
							  ILogger<CreateOrderHandler> logger)
	{
		_orderService = orderService;
		_logger = logger;
	}

	public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
	{
		var order = request.MapCreateOrderCommandToOrder();

		_logger.LogInformation("Creating order for customer {CustomerId}.", order.CustomerId);

		var result = await _orderService.AddOrder(order, cancellationToken);

		return result;
	}
}
