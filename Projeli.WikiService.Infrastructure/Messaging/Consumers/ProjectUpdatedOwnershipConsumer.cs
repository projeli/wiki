using MassTransit;
using Projeli.Shared.Application.Messages.Projects;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Infrastructure.Messaging.Consumers;

public class ProjectUpdatedOwnershipConsumer(IWikiService wikiService) : IConsumer<ProjectUpdatedOwnershipMessage>
{
    public async Task Consume(ConsumeContext<ProjectUpdatedOwnershipMessage> context)
    {
        var wiki = (await wikiService.GetByProjectId(context.Message.ProjectId, null, true)).Data;
        
        if (wiki is null) return;
        
        await wikiService.UpdateOwnership(wiki.Id, context.Message.FromUserId, context.Message.ToUserId, true);
    }
}