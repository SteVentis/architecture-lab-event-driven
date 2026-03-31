namespace OrderSystem.Contracts.Events;

public class OrderCreated
{
	public string EventId { get; set; } = string.Empty;
	public int OrderId { get; set; }
	public DateTime CreatedAt { get; set; }
	public decimal TotalAmount { get; set; }
	public int CustomerId { get; set; }
}
