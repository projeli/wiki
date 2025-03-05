using System.Reflection;
using MassTransit;
using MassTransit.Transports.Fabric;
using Projeli.Shared.Infrastructure.Exceptions;
using Projeli.Shared.Infrastructure.Messaging.Events;
using Projeli.WikiService.Infrastructure.Messaging.Consumers;

namespace Projeli.WikiService.Api.Extensions;

public static class RabbitMqExtension
{
    public static void UseWikiServiceRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(configuration["RabbitMq:Host"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Host"), "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Username"));
                    h.Password(configuration["RabbitMq:Password"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Password"));
                });

                config.ConfigureEndpoints(context);

                // config.ReceiveEndpoint<ProjectCreatedConsumer>("project-created-queue");
                // config.ReceiveEndpoint<ProjectSyncConsumer>("project-sync-queue");
                // config.ReceiveEndpoint<ProjectUpdatedConsumer>("project-updated-queue");
                
                config.ReceiveEndpoint("wiki-project-created-queue", e =>
                {
                    e.ConfigureConsumer<ProjectCreatedConsumer>(context);
                });
                
                config.ReceiveEndpoint("wiki-project-sync-queue", e =>
                {
                    e.ConfigureConsumer<ProjectSyncConsumer>(context);
                });
                
                config.ReceiveEndpoint("wiki-project-updated-queue", e =>
                {
                    e.ConfigureConsumer<ProjectUpdatedConsumer>(context);
                });

                config.PublishFanOut<ProjectSyncRequestEvent>();
            });

            x.AddConsumers(Assembly.GetAssembly(typeof(ProjectCreatedConsumer)));
        });
    }

    private static void ReceiveEndpoint<T>(this IRabbitMqBusFactoryConfigurator configurator, string queueName)
        where T : class, IConsumer, new()
    {
        configurator.ReceiveEndpoint("wiki-" + queueName, e => { e.Consumer<T>(); });
    }

    private static void PublishFanOut<T>(this IRabbitMqBusFactoryConfigurator configurator)
        where T : class
    {
        configurator.Publish<T>(y => y.ExchangeType = "fanout");
    }
}