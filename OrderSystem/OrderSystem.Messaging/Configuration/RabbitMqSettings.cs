namespace OrderSystem.Messaging.Configuration;

public class RabbitMqSettings
{
	public string HostName { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string VirtualHost { get; set; } = string.Empty;
	public int Port { get; set; }
	public string ClientProvidedName { get; set; } = string.Empty;
	public string ExchangeName { get; set; } = string.Empty;
	public string QueueName { get; set; } = string.Empty;
	public string RoutingKey { get; set; } = string.Empty;
}
