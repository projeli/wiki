using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Domain.Results;
using Projeli.Shared.Infrastructure.Extensions;
using Projeli.Shared.Infrastructure.Messaging.Events;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Requests;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Controllers.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/wikis")]
public class WikiController(IWikiService wikiService, IBus bus, IMapper mapper) : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWikiById([FromRoute] Ulid id)
    {
        var result = await wikiService.GetById(id, User.TryGetId());
        return HandleResult(result.Success
            ? new Result<WikiResponse>(mapper.Map<WikiResponse>(result.Data))
            : Result<WikiResponse>.NotFound());
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetWikiByProjectId([FromRoute] string projectId)
    {
        IResult<WikiDto?> result;

        if (Ulid.TryParse(projectId, out var ulid))
        {
            result = await TryGetWikiWithRetry(
                () => wikiService.GetByProjectId(ulid, User.TryGetId()),
                ulid,
                null); 
        }
        else
        {
            result = await TryGetWikiWithRetry(
                () => wikiService.GetByProjectSlug(projectId, User.TryGetId()),
                null,
                projectId);
        }

        return HandleResult(result.Success
            ? new Result<WikiResponse>(mapper.Map<WikiResponse>(result.Data))
            : Result<WikiResponse>.NotFound());
    }
    
    [HttpPut("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateWikiStatus([FromRoute] Ulid id, [FromBody] UpdateWikiStatusRequest request)
    {
        var result = await wikiService.UpdateStatus(id, request.Status, User.GetId());
        return HandleResult(result.Success
            ? new Result<WikiResponse>(mapper.Map<WikiResponse>(result.Data))
            : Result<WikiResponse>.NotFound());
    }
    
    [HttpPut("{id}/content")]
    [Authorize]
    public async Task<IActionResult> UpdateWikiContent([FromRoute] Ulid id, [FromBody] UpdateWikiContentRequest request)
    {
        var result = await wikiService.UpdateContent(id, request.Content, User.GetId());
        return HandleResult(result.Success
            ? new Result<WikiResponse>(mapper.Map<WikiResponse>(result.Data))
            : Result<WikiResponse>.NotFound());
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteWiki([FromRoute] Ulid id)
    {
        var result = await wikiService.Delete(id, User.GetId());
        return HandleResult(result.Success
            ? new Result<WikiResponse>(mapper.Map<WikiResponse>(result.Data))
            : Result<WikiResponse>.NotFound());
    }
    
    private async Task<IResult<WikiDto?>> TryGetWikiWithRetry(
        Func<Task<IResult<WikiDto?>>> getWikiFunc,  // Change to a factory function
        Ulid? projectId,
        string? projectSlug,
        int maxRetries = 5)
    {
        const int delaySeconds = 1;
        var attempts = 0;
        IResult<WikiDto?> result;

        do
        {
            result = await getWikiFunc();  // Create and await a new task each time

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