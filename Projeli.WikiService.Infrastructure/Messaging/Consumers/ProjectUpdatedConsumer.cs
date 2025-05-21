using MassTransit;
using Projeli.Shared.Domain.Results;
using Projeli.Shared.Infrastructure.Messaging.Events;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Infrastructure.Messaging.Consumers;

public class ProjectUpdatedConsumer(IWikiService wikiService) : IConsumer<ProjectUpdatedEvent>
{
    public async Task Consume(ConsumeContext<ProjectUpdatedEvent> context)
    {
        var wikiDto = new WikiDto
        {
            ProjectId = context.Message.ProjectId,
            ProjectName = context.Message.ProjectName,
            ProjectSlug = context.Message.ProjectSlug,
            Members = context.Message.Members.Select(x => new WikiMemberDto
            {
                UserId = x.UserId,
                IsOwner = x.IsOwner
            }).ToList()
        };

        var existingWiki = (await wikiService.GetByProjectId(wikiDto.ProjectId, null, true)).Data;

        if (existingWiki is null)
        {
            await wikiService.Create(wikiDto);
        } else {
            existingWiki.ProjectName = wikiDto.ProjectName;
            existingWiki.ProjectSlug = wikiDto.ProjectSlug;
            existingWiki.Members = wikiDto.Members;
            
            await wikiService.UpdateProjectInfo(existingWiki.Id, existingWiki);

            await wikiService.UpdateMembers(existingWiki.Id, existingWiki.Members, null, true);
        }
    }
}