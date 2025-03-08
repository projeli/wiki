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

    [HttpGet("project")]
    public async Task<IActionResult> GetPagesByProject([FromRoute] string wikiId)
    {
        IResult<List<PageDto>> result;
        if (Ulid.TryParse(wikiId, out var projectId))
        {
            result = await wikiPageService.GetByProjectId(projectId, User.TryGetId());
        }
        else
        {
            result = await wikiPageService.GetByProjectSlug(wikiId, User.TryGetId());
        }

        return HandleResult(result.Success
            ? new Result<List<SimplePageResponse>>(mapper.Map<List<SimplePageResponse>>(result.Data))
            : result);
    }

    [HttpGet("{pageId}")]
    public async Task<IActionResult> GetPage([FromRoute] Ulid wikiId, [FromRoute] string pageId)
    {
        IResult<PageDto?> result;
        if (Ulid.TryParse(pageId, out var ulid))
        {
            result = await wikiPageService.GetById(wikiId, ulid, User.TryGetId());
        }
        else
        {
            result = await wikiPageService.GetBySlug(wikiId, pageId, User.TryGetId());
        }

        return HandleResult(result.Success
            ? new Result<PageResponse>(mapper.Map<PageResponse>(result.Data))
            : result);
    }

    [HttpGet("{pageId}/project")]
    public async Task<IActionResult> GetPageByProject([FromRoute] string wikiId, [FromRoute] string pageId)
    {
        IResult<PageDto?> result;
        if (Ulid.TryParse(wikiId, out var projectId))
        {
            if (Ulid.TryParse(pageId, out var pageIdUlid))
            {
                result = await wikiPageService.GetByProjectIdAndId(projectId, pageIdUlid, User.TryGetId());
            }
            else
            {
                result = await wikiPageService.GetByProjectIdAndSlug(projectId, pageId, User.TryGetId());
            }
        }
        else
        {
            if (Ulid.TryParse(pageId, out var pageIdUlid))
            {
                result = await wikiPageService.GetByProjectSlugAndId(wikiId, pageIdUlid, User.TryGetId());
            }
            else
            {
                result = await wikiPageService.GetByProjectSlugAndSlug(wikiId, pageId, User.TryGetId());
            }
        }

        return HandleResult(result.Success
            ? new Result<PageResponse>(mapper.Map<PageResponse>(result.Data))
            : result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePage([FromRoute] Ulid wikiId, [FromBody] CreatePageRequest request)
    {
        var pageDto = mapper.Map<PageDto>(request);
        var result = await wikiPageService.Create(wikiId, pageDto, User.GetId());
        return HandleResult(result.Success
            ? new Result<PageResponse>(mapper.Map<PageResponse>(result.Data))
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
            ? new Result<PageResponse>(mapper.Map<PageResponse>(result.Data))
            : result);
    }

    [HttpPut("{pageId}/content")]
    [Authorize]
    public async Task<IActionResult> UpdatePageContent([FromRoute] Ulid wikiId, [FromRoute] Ulid pageId,
        [FromBody] UpdatePageContentRequest request)
    {
        var result = await wikiPageService.UpdateContent(wikiId, pageId, request.Content, User.GetId());
        return HandleResult(result.Success
            ? new Result<PageResponse>(mapper.Map<PageResponse>(result.Data))
            : result);
    }

    [HttpPut("{pageId}/categories")]
    [Authorize]
    public async Task<IActionResult> UpdatePageCategories([FromRoute] Ulid wikiId, [FromRoute] Ulid pageId,
        [FromBody] UpdatePageCategoriesRequest request)
    {
        var result = await wikiPageService.UpdateCategories(wikiId, pageId, request.CategoryIds, User.GetId());
        return HandleResult(result.Success
            ? new Result<PageResponse>(mapper.Map<PageResponse>(result.Data))
            : result);
    }

    [HttpPut("{pageId}/status")]
    [Authorize]
    public async Task<IActionResult> UpdatePageStatus([FromRoute] Ulid wikiId, [FromRoute] Ulid pageId,
        [FromBody] UpdatePageStatusRequest request)
    {
        var result = await wikiPageService.UpdateStatus(wikiId, pageId, request.Status, User.GetId());
        return HandleResult(result.Success
            ? new Result<PageResponse>(mapper.Map<PageResponse>(result.Data))
            : result);
    }

    [HttpDelete("{pageId}")]
    [Authorize]
    public async Task<IActionResult> DeletePage([FromRoute] Ulid wikiId, [FromRoute] Ulid pageId)
    {
        var result = await wikiPageService.Delete(wikiId, pageId, User.GetId());
        return HandleResult(result.Success
            ? new Result<PageResponse>(mapper.Map<PageResponse>(result.Data))
            : result);
    }
}