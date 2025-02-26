using MassTransit;
using Projeli.Shared.Infrastructure.Messaging.Events;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Infrastructure.Messaging.Consumers;

public class ProjectCreatedConsumer : IConsumer<ProjectCreatedEvent>
{
    public async Task Consume(ConsumeContext<ProjectCreatedEvent> context)
    {
        var wikiService = context.GetServiceOrCreateInstance<IWikiService>();

        var wikiDto = new WikiDto
        {
            ProjectId = context.Message.ProjectId,
            ProjectName = context.Message.ProjectName,
            ProjectSlug = context.Message.ProjectSlug,
            IsPublished = false,
            Members = context.Message.Members.Select(x => new MemberDto
            {
                UserId = x.UserId,
                IsOwner = x.IsOwner
            }).ToList()
        };
        
        var wikiResult = await wikiService.Create(wikiDto);
    }
}