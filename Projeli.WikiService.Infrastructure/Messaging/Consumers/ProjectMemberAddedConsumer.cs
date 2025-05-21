using MassTransit;
using Projeli.Shared.Application.Messages.Projects.Members;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Infrastructure.Messaging.Consumers;

public class ProjectMemberAddedConsumer(IWikiService wikiService, IWikiMemberService wikiMemberService) : IConsumer<ProjectMemberAddedMessage>
{
    public async Task Consume(ConsumeContext<ProjectMemberAddedMessage> context)
    {
        var wiki = (await wikiService.GetByProjectId(context.Message.ProjectId, null, true)).Data;
        
        if (wiki is null) return;
        
        await wikiMemberService.Add(wiki.Id, context.Message.UserId, context.Message.PerformingUserId, true);
    }
}