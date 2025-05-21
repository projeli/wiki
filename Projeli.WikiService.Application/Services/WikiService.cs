using AutoMapper;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public class WikiService(IWikiRepository repository, IMapper mapper) : IWikiService
{
    public async Task<IResult<WikiDto?>> GetById(Ulid id, string? userId, bool force = false)
    {
        var wiki = await repository.GetById(id, userId, force);
        return wiki is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(wiki))
            : Result<WikiDto?>.NotFound();
    }

    public async Task<IResult<WikiDto?>> GetByProjectId(Ulid projectId, string? userId, bool force = false)
    {
        var result = await repository.GetByProjectId(projectId, userId, force);
        return result is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(result))
            : Result<WikiDto?>.NotFound();
    }

    public async Task<IResult<WikiDto?>> GetByProjectSlug(string projectSlug, string? userId, bool force = false)
    {
        var result = await repository.GetByProjectSlug(projectSlug, userId, force);
        return result is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(result))
            : Result<WikiDto?>.NotFound();
    }

    public async Task<IResult<WikiStatisticsDto?>> GetStatistics(Ulid id, string? userId)
    {
        var result = await repository.GetStatistics(id, userId);
        return result is not null
            ? new Result<WikiStatisticsDto?>(mapper.Map<WikiStatisticsDto>(result))
            : Result<WikiStatisticsDto?>.NotFound();
    }

    public async Task<IResult<WikiDto?>> Create(WikiDto wikiDto)
    {
        wikiDto.Id = Ulid.NewUlid();
        wikiDto.Members.ForEach(member =>
        {
            member.Id = Ulid.NewUlid();
            member.WikiId = wikiDto.Id;
            member.Permissions = member.IsOwner ? WikiMemberPermissions.All : WikiMemberPermissions.None;
        });

        var validationResult = ValidateWiki(wikiDto);
        if (validationResult.Failed) return validationResult;

        var wiki = mapper.Map<Wiki>(wikiDto);
        var createdWiki = await repository.Create(wiki);

        return createdWiki is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(createdWiki))
            : Result<WikiDto?>.Fail("Failed to create wiki");
    }

    public async Task<IResult<WikiDto?>> UpdateProjectInfo(Ulid id, WikiDto wikiDto)
    {
        var existingWiki = await repository.GetById(id, null, true);
        if (existingWiki is null) return Result<WikiDto?>.NotFound();

        existingWiki.ProjectId = wikiDto.ProjectId;
        existingWiki.ProjectName = wikiDto.ProjectName;
        existingWiki.ProjectSlug = wikiDto.ProjectSlug;
        existingWiki.Members = wikiDto.Members.Select(member => new WikiMember
        {
            Id = Ulid.NewUlid(),
            WikiId = existingWiki.Id,
            UserId = member.UserId,
            IsOwner = member.IsOwner,
            Permissions = member.IsOwner ? WikiMemberPermissions.All : WikiMemberPermissions.None
        }).ToList();

        var updatedWiki = await repository.Update(existingWiki);

        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }

    public async Task<IResult<WikiDto?>> UpdateStatus(Ulid id, WikiStatus status, string userId)
    {
        var existingWiki = await repository.GetById(id, userId);
        if (existingWiki is null) return Result<WikiDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
        if (member is null || (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWiki)))
        {
            throw new ForbiddenException("You do not have permission to edit this wiki");
        }

        switch (status)
        {
            case WikiStatus.Uncreated:
                return Result<WikiDto>.Fail("Cannot set status to uncreated");
            case WikiStatus.Draft when existingWiki.Status != WikiStatus.Uncreated:
                return Result<WikiDto>.Fail("Cannot set status to draft");
            case WikiStatus.Published
                when existingWiki.Status != WikiStatus.Draft && existingWiki.Status != WikiStatus.Archived:
                return Result<WikiDto>.Fail("Cannot set status to published");
            case WikiStatus.Archived when existingWiki.Status != WikiStatus.Published:
                return Result<WikiDto>.Fail("Cannot set status to archived");
        }

        var updatedWiki = await repository.UpdateStatus(id, status);

        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }

    public async Task<IResult<WikiDto?>> UpdateContent(Ulid id, string content, string userId)
    {
        var existingWiki = await repository.GetById(id, userId);
        if (existingWiki is null) return Result<WikiDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
        if (member is null || (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWiki)))
        {
            throw new ForbiddenException("You do not have permission to edit this wiki");
        }

        var updatedWiki = await repository.UpdateContent(id, content);
        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }

    public async Task<IResult<WikiDto?>> UpdateSidebar(Ulid id, WikiConfigDto.WikiConfigSidebarDto sidebar,
        string userId)
    {
        var existingWiki = await repository.GetById(id, userId);
        if (existingWiki is null) return Result<WikiDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
        if (member is null || (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWiki)))
        {
            throw new ForbiddenException("You do not have permission to edit this wiki");
        }

        for (var i = sidebar.Items.Count - 1; i >= 0; i--)
        {
            var item = sidebar.Items[i];

            if (item.Slug is not null)
            {
                item.Category = null;
            }
            else if (item.Category is not null)
            {
                item.Slug = null;
                item.Index = Ulid.NewUlid().ToString();

                if (item.Category != null)
                {
                    for (var j = item.Category.Count - 1; j >= 0; j--)
                    {
                        var subItem = item.Category[j];
                        if (subItem.Slug is not null)
                        {
                            subItem.Category = null;
                        }
                        else
                        {
                            item.Category.RemoveAt(j);
                        }
                    }
                }
            }
            else
            {
                sidebar.Items.RemoveAt(i);
            }
        }

        var updatedWiki = await repository.UpdateSidebar(id, mapper.Map<WikiConfig.WikiConfigSidebar>(sidebar));

        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }

    public async Task<IResult<WikiDto?>> UpdateMembers(Ulid id, List<WikiMemberDto> members, string userId,
        bool force = false)
    {
        var existingWiki = await repository.GetById(id, userId, force);
        if (existingWiki is null) return Result<WikiDto?>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
        if (!force && (member is null ||
                       (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWiki))))
        {
            throw new ForbiddenException("You do not have permission to edit this wiki");
        }

        var existingWikiMemberIds = existingWiki.Members.Select(x => x.UserId);
        var newUserIds = members
            .Where(x => !existingWikiMemberIds.Contains(x.UserId))
            .Select(x => x.UserId);
        var removedUserIds = existingWiki.Members
            .Where(x => !members.Select(m => m.UserId).Contains(x.UserId))
            .Select(x => x.UserId).ToList();

        var newMembers = members
            .Where(x => newUserIds.Contains(x.UserId))
            .Select(x => new WikiMember
            {
                Id = Ulid.NewUlid(),
                WikiId = existingWiki.Id,
                UserId = x.UserId,
                IsOwner = x.IsOwner,
                Permissions = x.IsOwner ? WikiMemberPermissions.All : WikiMemberPermissions.None
            });

        await repository.AddMembers(id, newMembers.ToList());
        await repository.RemoveMembers(id, removedUserIds);

        var existingMembers = existingWiki.Members
            .Where(x => !removedUserIds.Contains(x.UserId));

        var oldOwner = existingMembers.FirstOrDefault(x => x.IsOwner) ??
                       throw new ArgumentException("Wiki must have an owner");

        var newOwner = members.FirstOrDefault(x => x.IsOwner) ?? mapper.Map<WikiMemberDto>(oldOwner);

        if (oldOwner.UserId != newOwner.UserId)
        {
            var oldOwnerPermissions = Enum.GetValues(typeof(WikiMemberPermissions))
                .Cast<WikiMemberPermissions>()
                .Where(p => p != WikiMemberPermissions.All)
                .Aggregate(WikiMemberPermissions.None, (current, permission) => current | permission);
            
            await repository.UpdateOwnership(id, oldOwner.UserId, newOwner.UserId, oldOwnerPermissions,
                WikiMemberPermissions.All);
        }
        
        return new Result<WikiDto>(mapper.Map<WikiDto>(await repository.GetById(id, userId, force)));
    }

    public async Task<IResult<WikiDto?>> Delete(Ulid id, string userId)
    {
        var existingWiki = await repository.GetById(id, userId);
        if (existingWiki is null) return Result<WikiDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
        if (member is null || (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.DeleteWiki)))
        {
            throw new ForbiddenException("You do not have permission to delete this wiki");
        }

        var success = await repository.Delete(id);
        return success
            ? new Result<WikiDto>(mapper.Map<WikiDto>(existingWiki))
            : Result<WikiDto>.Fail("Failed to delete project");
    }

    private static IResult<WikiDto?> ValidateWiki(WikiDto wikiDto)
    {
        var errors = new Dictionary<string, string[]>();

        if (wikiDto.Id == Ulid.Empty)
        {
            errors.Add(nameof(wikiDto.Id), ["Id is required"]);
        }

        if (wikiDto.ProjectId == Ulid.Empty)
        {
            errors.Add(nameof(wikiDto.ProjectId), ["ProjectId is required"]);
        }

        if (string.IsNullOrWhiteSpace(wikiDto.ProjectSlug))
        {
            errors.Add(nameof(wikiDto.ProjectSlug), ["ProjectSlug is required"]);
        }

        if (wikiDto.Members.Count == 0)
        {
            errors.Add(nameof(wikiDto.Members), ["Members are required"]);
        }

        return errors.Count > 0
            ? Result<WikiDto?>.ValidationFail(errors)
            : new Result<WikiDto?>(null);
    }
}