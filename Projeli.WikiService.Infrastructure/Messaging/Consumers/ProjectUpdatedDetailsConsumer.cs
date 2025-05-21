using MassTransit;
using Projeli.Shared.Application.Messages.Projects;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Infrastructure.Messaging.Consumers;

public class ProjectUpdatedDetailsConsumer(IWikiService wikiService)
    : IConsumer<ProjectUpdatedDetailsMessage>
{
    public async Task Consume(ConsumeContext<ProjectUpdatedDetailsMessage> context)
    {
        var existingWiki = (await wikiService.GetByProjectId(context.Message.ProjectId, null, true)).Data;

        if (existingWiki is not null)
        {
            existingWiki.ProjectName = context.Message.ProjectName;
            existingWiki.ProjectSlug = context.Message.ProjectSlug;
            
            await wikiService.UpdateProjectDetails(existingWiki.Id, existingWiki);
        }
    }
}