using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Common;
using OrderSystem.Api.CQRS.Commands;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/create")]
    public async Task<Result<int>> CreateOderAsync([FromBody] CreateOrderCommand orderCommand)
    {
        var result = await _mediator.Send(orderCommand);

        return result;
    }
}
