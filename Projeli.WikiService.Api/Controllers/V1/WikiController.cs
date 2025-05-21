using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Domain.Results;
using Projeli.Shared.Infrastructure.Extensions;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Requests;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Api.Controllers.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/wikis")]
public class WikiController(IWikiService wikiService, IMapper mapper) : BaseController
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
            result = await wikiService.GetByProjectId(ulid, User.TryGetId());
        }
        else
        {
            result = await wikiService.GetByProjectSlug(projectId, User.TryGetId());
        }

        return HandleResult(result.Success
            ? new Result<WikiResponse>(mapper.Map<WikiResponse>(result.Data))
            : Result<WikiResponse>.NotFound());
    }

    [HttpGet("{id}/statistics")]
    public async Task<IActionResult> GetWikiStatistics([FromRoute] Ulid id)
    {
        var result = await wikiService.GetStatistics(id, User.TryGetId());
        return HandleResult(result.Success
            ? new Result<WikiStatisticsResponse>(mapper.Map<WikiStatisticsResponse>(result.Data))
            : Result<WikiStatisticsResponse>.NotFound());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateWiki([FromBody] CreateWikiRequest request)
    {
        var result = await wikiService.Create(
            request.ProjectId,
            request.ProjectName,
            request.ProjectSlug,
            request.Members.Select(member => new WikiMemberDto
            {
                UserId = member.UserId,
                IsOwner = member.IsOwner,
            }).ToList(),
            User.GetId()
        );

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

    [HttpPut("{id}/sidebar")]
    [Authorize]
    public async Task<IActionResult> UpdateWikiSidebar([FromRoute] Ulid id, [FromBody] UpdateWikiSidebarRequest request)
    {
        var result = await wikiService.UpdateSidebar(id, request.Sidebar, User.GetId());
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
}