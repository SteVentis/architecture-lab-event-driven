namespace OrderSystem.Messaging.Publisher;

public interface IRabbitMqPublisher
{
	public Task PublishAsync(string message, CancellationToken cancellationToken = default);
}
