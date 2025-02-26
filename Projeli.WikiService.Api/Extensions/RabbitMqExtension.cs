using System.Reflection;
using MassTransit;
using MassTransit.Transports.Fabric;
using Projeli.Shared.Infrastructure.Messaging.Events;
using Projeli.WikiService.Infrastructure.Messaging.Consumers;

namespace Projeli.WikiService.Api.Extensions;

public static class RabbitMqExtension
{
    public static void UseWikiServiceRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, config) =>
            {
                config.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                config.ConfigureEndpoints(context);

                config.ReceiveEndpoint<ProjectCreatedConsumer>("project-created-queue");
                config.ReceiveEndpoint<ProjectSyncConsumer>("project-sync-queue");
                config.ReceiveEndpoint<ProjectUpdatedConsumer>("project-updated-queue");

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