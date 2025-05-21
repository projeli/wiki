using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiMemberService
{
    Task<IResult<WikiMemberDto?>> Add(Ulid wikiId, string userId, string performingUserId, bool force = false);
    Task<IResult<WikiMemberDto?>> UpdatePermissions(Ulid wikiId, Ulid userId,
        WikiMemberPermissions requestPermissions, string performingUserId);
    Task<IResult<WikiMemberDto?>> Delete(Ulid wikiId, string userId, string performingUserId, bool force = false);
}