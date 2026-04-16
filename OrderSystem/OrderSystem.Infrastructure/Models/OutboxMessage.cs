namespace OrderSystem.Infrastructure.Models;

public class OutboxMessage
{
	public int Id { get; set; }
	public string EventId { get; set; } = string.Empty;
	public string EventType { get; set; } = string.Empty;
	public string Payload { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime? SentAt { get; set; }
	public OutboxMessageStatus MessageStatus { get; set; }
}

public enum OutboxMessageStatus
{
	NotSent = 0,
	Sent = 1
}