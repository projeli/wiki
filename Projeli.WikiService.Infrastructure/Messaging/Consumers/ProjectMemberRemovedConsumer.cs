using MassTransit;
using Projeli.Shared.Application.Messages.Projects.Members;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Infrastructure.Messaging.Consumers;

public class ProjectMemberRemovedConsumer(IWikiService wikiService, IWikiMemberService wikiMemberService) : IConsumer<ProjectMemberRemovedMessage>
{
    public async Task Consume(ConsumeContext<ProjectMemberRemovedMessage> context)
    {
        var wiki = (await wikiService.GetByProjectId(context.Message.ProjectId, null, true)).Data;
        
        if (wiki is null) return;
        
        await wikiMemberService.Delete(wiki.Id, context.Message.UserId, context.Message.PerformingUserId, true);
    }
}