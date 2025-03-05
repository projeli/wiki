using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Domain.Results;
using Projeli.Shared.Infrastructure.Extensions;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Requests;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Controllers.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/wikis/{wikiId}/pages")]
public class WikiPageController(IWikiPageService wikiPageService, IMapper mapper) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetPages([FromRoute] Ulid wikiId)
    {
        var result = await wikiPageService.GetByWikiId(wikiId, User.TryGetId());
        return HandleResult(result.Success
            ? new Result<List<SimplePageResponse>>(mapper.Map<List<SimplePageResponse>>(result.Data))
            : result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePage([FromRoute] Ulid wikiId, [FromBody] CreatePageRequest request)
    {
        var pageDto = mapper.Map<PageDto>(request);
        var result = await wikiPageService.Create(wikiId, pageDto, User.GetId());
        return HandleResult(result.Success
            ? new Result<SimplePageResponse>(mapper.Map<SimplePageResponse>(result.Data))
            : result);
    }

    [HttpPut("{pageId}")]
    [Authorize]
    public async Task<IActionResult> UpdatePage([FromRoute] Ulid wikiId, [FromRoute] Ulid pageId,
        [FromBody] UpdatePageRequest request)
    {
        var pageDto = mapper.Map<PageDto>(request);
        var result = await wikiPageService.Update(wikiId, pageId, pageDto, User.GetId());
        return HandleResult(result.Success
            ? new Result<SimplePageResponse>(mapper.Map<SimplePageResponse>(result.Data))
            : result);
    }

    [HttpDelete("{pageId}")]
    [Authorize]
    public async Task<IActionResult> DeletePage([FromRoute] Ulid wikiId, [FromRoute] Ulid pageId)
    {
        var result = await wikiPageService.Delete(wikiId, pageId, User.GetId());
        return HandleResult(result.Success
            ? new Result<SimplePageResponse>(mapper.Map<SimplePageResponse>(result.Data))
            : result);
    }
}