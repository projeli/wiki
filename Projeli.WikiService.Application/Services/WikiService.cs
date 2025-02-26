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

    public async Task<IResult<WikiDto?>> Create(WikiDto wikiDto)
    {
        wikiDto.Id = Ulid.NewUlid();
        wikiDto.CreatedAt = DateTime.UtcNow;
        wikiDto.Members.ForEach(member =>
        {
            member.Id = Ulid.NewUlid();
            member.WikiId = wikiDto.Id;
            member.Permissions = member.IsOwner ? WikiMemberPermissions.All : WikiMemberPermissions.None;
        });

        if (wikiDto.IsPublished)
        {
            wikiDto.PublishedAt = DateTime.UtcNow;
        }
        
        var validationResult = ValidateWiki(wikiDto);
        if (validationResult.Failed) return validationResult;
        
        var wiki = mapper.Map<Wiki>(wikiDto);
        var createdWiki = await repository.Create(wiki);
        
        return createdWiki is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(createdWiki))
            : Result<WikiDto?>.Fail("Failed to create wiki");
        
    }

    public async Task<IResult<WikiDto?>> Update(Ulid id, WikiDto wikiDto, string? userId, bool force = false)
    {
        var existingWiki = await repository.GetById(id, userId, force);
        if (existingWiki is null) return Result<WikiDto?>.NotFound();

        if (userId is not null)
        {
            var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
            if (member is null || !member.IsOwner || !member.Permissions.HasFlag(WikiMemberPermissions.EditWiki))
            {
                throw new ForbiddenException("You do not have permission to edit this wiki");
            }
        }
        
        wikiDto.Id = existingWiki.Id;
        wikiDto.CreatedAt = existingWiki.CreatedAt;
        wikiDto.UpdatedAt = DateTime.UtcNow;
        
        if (wikiDto.IsPublished && (!existingWiki.IsPublished || existingWiki.PublishedAt is null))
        {
            wikiDto.PublishedAt = DateTime.UtcNow;
        }
        
        var validationResult = ValidateWiki(wikiDto);
        if (validationResult.Failed) return validationResult;
        
        var wiki = mapper.Map<Wiki>(wikiDto);
        var updatedWiki = await repository.Update(wiki);
        
        return updatedWiki is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto?>.Fail("Failed to update wiki");
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