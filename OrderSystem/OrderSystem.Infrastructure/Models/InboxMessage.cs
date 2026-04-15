namespace OrderSystem.Infrastructure.Models;

public class InboxMessage
{
	public int Id { get; set; }
	public string EventId { get; set; } = string.Empty;
	public string EventType { get; set; } = string.Empty;
	public DateTime ReceivedAt { get; set; }
	public DateTime? ProcessedAt { get; set; }
	public InboxMessageStatus MessageStatus { get; set; }
}

public enum InboxMessageStatus
{
	Received,
	Processed,
	NotProcessed
}
