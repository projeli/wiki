using System.Security.Principal;
using MassTransit;
using Projeli.Shared.Infrastructure.Messaging.Events;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Infrastructure.Messaging.Consumers;

public class ProjectDeletedConsumer(IWikiService wikiService) : IConsumer<ProjectDeletedEvent>
{
    public async Task Consume(ConsumeContext<ProjectDeletedEvent> context)
    {
        var wiki = await wikiService.GetByProjectId(context.Message.ProjectId, null, true);

        if (wiki is { Success: true, Data: not null })
        {
            var wikiOwner = wiki.Data.Members.FirstOrDefault(m => m.IsOwner);
            if (wikiOwner is not null)
            {
                await wikiService.Delete(wiki.Data.Id, wikiOwner.UserId);
            }
        }
    }
}