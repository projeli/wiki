using MassTransit;
using Projeli.Shared.Application.Messages.Projects;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Infrastructure.Messaging.Consumers;

public class ProjectDeletedConsumer(IWikiService wikiService) : IConsumer<ProjectDeletedMessage>
{
    public async Task Consume(ConsumeContext<ProjectDeletedMessage> context)
    {
        var wiki = await wikiService.GetByProjectId(context.Message.ProjectId, null, true);

        if (wiki is { Success: true, Data: not null })
        {
            await wikiService.Delete(wiki.Data.Id, null, true);
        }
    }
}