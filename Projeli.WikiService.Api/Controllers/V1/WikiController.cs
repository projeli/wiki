using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Domain.Results;
using Projeli.Shared.Infrastructure.Extensions;
using Projeli.Shared.Infrastructure.Messaging.Events;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Controllers.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/wikis")]
public class WikiController(IWikiService wikiService, IBus bus) : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWikiById([FromRoute] Ulid id)
    {
        var result = await wikiService.GetById(id, User.TryGetId());
        return HandleResult(result);
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetWikiByProjectId([FromRoute] string projectId)
    {
        IResult<WikiDto?> result;

        if (Ulid.TryParse(projectId, out var ulid))
        {
            result = await TryGetWikiWithRetry(wikiService.GetByProjectId(ulid, User.TryGetId()), ulid, null);
        }
        else
        {
            result = await TryGetWikiWithRetry(wikiService.GetByProjectSlug(projectId, User.TryGetId()), null, projectId);
        }

        return HandleResult(result);
    }

    private async Task<IResult<WikiDto?>> TryGetWikiWithRetry(
        Task<IResult<WikiDto?>> getWikiTask,
        Ulid? projectId,
        string? projectSlug,
        int maxRetries = 5)
    {
        const int delaySeconds = 1;
        var attempts = 0;
        IResult<WikiDto?> result;

        do
        {
            result = await getWikiTask;

            if (result.Data != null)
            {
                return result;
            }

            if (attempts < maxRetries)
            {
                if (attempts == 0)
                {
                    // Publish sync request only on first failed attempt
                    await bus.Publish(new ProjectSyncRequestEvent
                    {
                        ProjectId = projectId,
                        ProjectSlug = projectSlug
                    });
                }

                // Wait before retrying
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }

            attempts++;
        } while (attempts <= maxRetries);

        return result;
    }
}