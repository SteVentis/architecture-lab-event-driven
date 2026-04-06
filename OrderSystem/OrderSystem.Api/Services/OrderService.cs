using OrderSystem.Api.Common;
using OrderSystem.Api.Mapping;
using OrderSystem.Infrastructure.Context;
using OrderSystem.Infrastructure.Models;

namespace OrderSystem.Api.Services;

public interface IOrderService
{
	Task<Result<int>> AddOrder(Order order, CancellationToken cancellationToken);
}

internal sealed class OrderService : IOrderService
{
	private readonly AppDbContext _dbContext;
	public OrderService(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Result<int>> AddOrder(Order order, CancellationToken cancellationToken)
	{
		await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

		try
		{
			order.Status = Status.Pending;

		    await _dbContext.Orders.AddAsync(order, cancellationToken);

			var orderCreatedEvent = order.ToOrderCreatedEvent();

			var outboxMessage = orderCreatedEvent.ToOutboxMessage();

			await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

			await _dbContext.SaveChangesAsync(cancellationToken);

			await transaction.CommitAsync(cancellationToken);

			return Result<int>.Success(order.Id);
		}
		catch (OperationCanceledException)
		{
			await transaction.RollbackAsync();
			throw;
		}
		catch (Exception ex)
		{
			await transaction.RollbackAsync();

			return Result<int>.Failure(ex.Message);
		}
	}
}
