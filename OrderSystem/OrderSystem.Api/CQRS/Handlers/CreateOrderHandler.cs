using MediatR;
using OrderSystem.Api.Common;
using OrderSystem.Api.CQRS.Commands;

namespace OrderSystem.Api.CQRS.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result>
{
	public Task<Result> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
