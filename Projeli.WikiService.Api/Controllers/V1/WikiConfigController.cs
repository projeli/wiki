using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Infrastructure.Extensions;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Controllers.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/wiki/{id}/config")]
public class WikiConfigController(IWikiConfigService wikiConfigService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetWikiConfig([FromRoute] Ulid id)
    {
        var wikiConfig = await wikiConfigService.GetByWikiId(id, User.TryGetId());
        return HandleResult(wikiConfig);
    }
}