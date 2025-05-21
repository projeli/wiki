using MassTransit;
using Projeli.Shared.Infrastructure.Exceptions;
using Projeli.WikiService.Infrastructure.Messaging.Consumers;

namespace Projeli.WikiService.Api.Extensions;

public static class RabbitMqExtension
{
    public static void UseWikiServiceRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProjectUpdatedDetailsConsumer>();
            x.AddConsumer<ProjectUpdatedOwnershipConsumer>();
            x.AddConsumer<ProjectMemberAddedConsumer>();
            x.AddConsumer<ProjectMemberRemovedConsumer>();
            x.AddConsumer<ProjectDeletedConsumer>();
            
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(configuration["RabbitMq:Host"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Host"), "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Username"));
                    h.Password(configuration["RabbitMq:Password"] ?? throw new MissingEnvironmentVariableException("RabbitMq:Password"));
                });
                
                config.ReceiveEndpoint("wiki-project-updated-details-queue", e =>
                {
                    e.ConfigureConsumer<ProjectUpdatedDetailsConsumer>(context);
                });
                
                config.ReceiveEndpoint("wiki-project-updated-ownership-queue", e =>
                {
                    e.ConfigureConsumer<ProjectUpdatedOwnershipConsumer>(context);
                });
                
                config.ReceiveEndpoint("wiki-project-member-added-queue", e =>
                {
                    e.ConfigureConsumer<ProjectMemberAddedConsumer>(context);
                });
                
                config.ReceiveEndpoint("wiki-project-member-removed-queue", e =>
                {
                    e.ConfigureConsumer<ProjectMemberRemovedConsumer>(context);
                });
                
                config.ReceiveEndpoint("wiki-project-deleted-queue", e =>
                {
                    e.ConfigureConsumer<ProjectDeletedConsumer>(context);
                });
            });
        });
    }

    private static void PublishFanOut<T>(this IRabbitMqBusFactoryConfigurator configurator)
        where T : class
    {
        configurator.Publish<T>(y => y.ExchangeType = "fanout");
    }
}