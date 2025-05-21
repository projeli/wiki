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
            x.AddConsumer<ProjectCreatedConsumer>();
            x.AddConsumer<ProjectSyncConsumer>();
            x.AddConsumer<ProjectUpdatedConsumer>();
            x.AddConsumer<ProjectDeletedConsumer>();
            
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(configuration["RabbitMq:Host"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Host"), "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Username"));
                    h.Password(configuration["RabbitMq:Password"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Password"));
                });
                
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
                
                config.ReceiveEndpoint("wiki-project-deleted-queue", e =>
                {
                    e.ConfigureConsumer<ProjectDeletedConsumer>(context);
                });

                config.PublishFanOut<ProjectSyncRequestEvent>();
            });
        });
    }

    private static void PublishFanOut<T>(this IRabbitMqBusFactoryConfigurator configurator)
        where T : class
    {
        configurator.Publish<T>(y => y.ExchangeType = "fanout");
    }
}