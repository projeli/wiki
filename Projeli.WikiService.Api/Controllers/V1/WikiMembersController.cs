using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Domain.Results;
using Projeli.Shared.Infrastructure.Extensions;
using Projeli.WikiService.Application.Models.Requests;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Controllers.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/wikis/{wikiId}/members")]
public class WikiMembersController(IWikiMemberService wikiMemberService, IMapper mapper) : BaseController
{
    [HttpPut("{userId}/permissions")]
    [Authorize]
    public async Task<IActionResult> UpdateWikiMemberPermissions([FromRoute] Ulid wikiId, [FromRoute] Ulid userId,
        [FromBody] UpdateWikiMemberPermissionsRequest request)
    {
        var result = await wikiMemberService.UpdatePermissions(wikiId, userId, request.Permissions, User.GetId());
        return HandleResult(result.Success
            ? new Result<WikiMemberResponse>(mapper.Map<WikiMemberResponse>(result.Data))
            : Result<WikiMemberResponse>.NotFound());
    }
}