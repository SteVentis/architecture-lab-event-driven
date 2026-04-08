using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Messaging.Configuration;
using OrderSystem.Messaging.Connection;
using OrderSystem.Messaging.Publisher;
using OrderSystem.Messaging.Topology;

namespace OrderSystem.Messaging;

public static class ServiceCollection
{
	public static void SetupMessaging(this IServiceCollection services, Action<RabbitMqSettings> configure)
	{
		services.Configure(configure);
		services.AddSingleton<IRabbitMqConnectionProvider, RabbitMqConnectionProvider>();
		services.AddSingleton<IRabbitMqTopologyInitializer, RabbitMqTopologyInitializer>();
		services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

		services.AddHostedService<RabbitMqStartupInitializer>();
	}

	
}
