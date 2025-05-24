using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Infrastructure.Extensions;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Controllers.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/wikis/{wikiId}/events")]
public class WikiEventsController(IWikiEventService wikiEventService) : BaseController
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetWikiEvents(
        [FromRoute] Ulid wikiId,
        [FromQuery] List<string>? userIds,
        [FromQuery] List<string>? eventTypes,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        return HandleResult(await wikiEventService.GetEvents(
            wikiId,
            userIds ?? [],
            eventTypes ?? [],
            page,
            pageSize,
            User.GetId())
        );
    }
}