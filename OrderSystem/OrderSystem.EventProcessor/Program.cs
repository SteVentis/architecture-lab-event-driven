using NLog.Extensions.Logging;
using OrderSystem.EventProcessor;
using OrderSystem.Infrastructure.Context;
using OrderSystem.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.SetupMessaging(settings =>
{
	builder.Configuration.GetSection("RabbitMqSettings").Bind(settings);
});

builder.Services.AddSqlServer<AppDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Logging.AddNLog();

builder.Services.AddHostedService<EventConsumer>();

var host = builder.Build();
host.Run();
