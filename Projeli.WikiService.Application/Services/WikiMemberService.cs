using AutoMapper;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public class WikiMemberService(
    IWikiRepository wikiRepository,
    IWikiMemberRepository wikiMemberRepository,
    IMapper mapper)
    : IWikiMemberService
{
    public async Task<IResult<WikiMemberDto?>> UpdatePermissions(Ulid wikiId, Ulid userId,
        WikiMemberPermissions requestPermissions, string performingUserId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, performingUserId);
        if (existingWiki is null) return Result<WikiMemberDto>.NotFound();

        var performingMember = existingWiki.Members.FirstOrDefault(member => member.UserId == performingUserId);
        if (performingMember is null || (!performingMember.IsOwner &&
                                         !performingMember.Permissions.HasFlag(WikiMemberPermissions
                                             .EditWikiMemberPermissions)))
        {
            throw new ForbiddenException("You do not have permission to edit wiki member permissions.");
        }

        var memberToUpdate = existingWiki.Members.FirstOrDefault(member => member.Id == userId);

        if (memberToUpdate is null)
        {
            return Result<WikiMemberDto>.NotFound();
        }

        if (memberToUpdate.IsOwner)
        {
            throw new ForbiddenException(
                "You cannot change the permissions of the owner of the wiki.");
        }

        var difference = requestPermissions ^ memberToUpdate.Permissions;
        if (difference != WikiMemberPermissions.None && !performingMember.Permissions.HasFlag(difference))
        {
            throw new ForbiddenException("You can only add permissions that you have.");
        }

        memberToUpdate.Permissions = requestPermissions;

        var result = await wikiMemberRepository.UpdatePermissions(wikiId, userId, requestPermissions);
        return result is not null
            ? new Result<WikiMemberDto>(mapper.Map<WikiMemberDto>(memberToUpdate))
            : Result<WikiMemberDto>.Fail("Failed to update project member permissions");
    }
}