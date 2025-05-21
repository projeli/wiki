using AutoMapper;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Models.Events.Members;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public class WikiMemberService(
    IWikiRepository wikiRepository,
    IWikiMemberRepository wikiMemberRepository,
    IEventRepository eventRepository,
    IMapper mapper)
    : IWikiMemberService
{
    public async Task<IResult<WikiMemberDto?>> Add(Ulid wikiId, string userId, string performingUserId, bool force = false)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, performingUserId, force);
        if (existingWiki is null) return Result<WikiMemberDto>.NotFound();

        var newMember = await wikiMemberRepository.Add(wikiId, new WikiMember
        {
            Id = Ulid.NewUlid(),
            WikiId = wikiId,
            UserId = userId,
            Permissions = WikiMemberPermissions.None,
            IsOwner = false
        });

        if (newMember is not null)
        {
            await eventRepository.StoreEvent(wikiId, new WikiMemberAddedEvent
            {
                UserId = performingUserId,
                MemberId = userId,
            });
        }
        
        return newMember is not null
            ? new Result<WikiMemberDto>(mapper.Map<WikiMemberDto>(newMember))
            : Result<WikiMemberDto>.Fail("Failed to add project member");
    }

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
        
        if (difference == WikiMemberPermissions.None)
        {
            return new Result<WikiMemberDto>(mapper.Map<WikiMemberDto>(memberToUpdate));
        }

        memberToUpdate.Permissions = requestPermissions;

        var updatedMember = await wikiMemberRepository.UpdatePermissions(wikiId, userId, requestPermissions);

        if (updatedMember is not null)
        {
            await eventRepository.StoreEvent(wikiId, new WikiMemberUpdatedPermissionsEvent
            {
                UserId = performingUserId,
                MemberId = updatedMember.UserId,
                Permissions = updatedMember.Permissions,
            });
        }
        
        return updatedMember is not null
            ? new Result<WikiMemberDto>(mapper.Map<WikiMemberDto>(memberToUpdate))
            : Result<WikiMemberDto>.Fail("Failed to update project member permissions");
    }

    public async Task<IResult<WikiMemberDto?>> Delete(Ulid wikiId, string userId, string performingUserId, bool force = false)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, performingUserId, force);
        if (existingWiki is null) return Result<WikiMemberDto>.NotFound();

        var memberToDelete = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);

        if (memberToDelete is null)
        {
            return Result<WikiMemberDto>.NotFound();
        }

        var success = await wikiMemberRepository.Delete(wikiId, userId);

        if (success)
        {
            await eventRepository.StoreEvent(wikiId, new WikiMemberRemovedEvent
            {
                UserId = performingUserId,
                MemberId = userId,
            });
        }
        
        return success
            ? new Result<WikiMemberDto>(mapper.Map<WikiMemberDto>(success))
            : Result<WikiMemberDto>.Fail("Failed to delete project member");
    }
}