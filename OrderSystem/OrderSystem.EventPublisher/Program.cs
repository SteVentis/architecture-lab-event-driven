using OrderSystem.EventPublisher;
using OrderSystem.Messaging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();


builder.Services.SetupMessaging(settings => 
{
	builder.Configuration.GetSection("RabbitMqSettings").Bind(settings);
});


var host = builder.Build();
host.Run();
