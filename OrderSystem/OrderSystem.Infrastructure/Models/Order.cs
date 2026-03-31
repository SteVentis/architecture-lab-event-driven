namespace OrderSystem.Infrastructure.Models;

public class Order
{
	public int Id { get; set; }
	public string ProductName { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public Status Status { get; set; }
	public decimal TotalAmount { get; set; }
	public int CustomerId { get; set; }
}

public enum Status
{
	Created,
	Processing,
	Completed,
	Cancelled
}
