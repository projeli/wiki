using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiMemberService
{
    Task<IResult<WikiMemberDto?>> UpdatePermissions(Ulid wikiId, Ulid userId,
        WikiMemberPermissions requestPermissions, string performingUserId);
}