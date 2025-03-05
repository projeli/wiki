using System.Text.RegularExpressions;
using AutoMapper;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public partial class WikiPageService(IWikiPageRepository wikiPageRepository, IWikiRepository wikiRepository, IMapper mapper) : IWikiPageService
{
    public async Task<IResult<List<PageDto>>> GetByWikiId(Ulid wikiId, string? userId = null, bool force = false)
    {
        var pages = await wikiPageRepository.GetByWikiId(wikiId, userId, force);
        return new Result<List<PageDto>>(mapper.Map<List<PageDto>>(pages));
    }

    public async Task<IResult<PageDto?>> Create(Ulid wikiId, PageDto pageDto, string userId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, userId);
        if (existingWiki is null) return Result<PageDto>.NotFound();
        
        var member = existingWiki.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null ||
            (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.CreateWikiPages)))
        {
            throw new ForbiddenException("You do not have permission to create pages for this wiki.");
        }
        
        var page = mapper.Map<Page>(pageDto);
        
        page.Id = Ulid.NewUlid();
        page.CreatedAt = DateTime.UtcNow;
        
        var validationResult = await ValidatePage(page);
        if (!validationResult.Success) return validationResult;
        
        var newPage = await wikiPageRepository.Create(wikiId, page);
        
        return newPage is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(newPage))
            : Result<PageDto>.Fail("Failed to create page.");
    }

    public async Task<IResult<PageDto?>> Update(Ulid wikiId, Ulid pageId, PageDto pageDto, string userId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, userId);
        if (existingWiki is null) return Result<PageDto>.NotFound();
        
        var member = existingWiki.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null ||
            (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWikiPages)))
        {
            throw new ForbiddenException("You do not have permission to update pages for this wiki.");
        }
        
        var existingPage = await wikiPageRepository.GetById(wikiId, pageId, userId);
        if (existingPage is null) return Result<PageDto>.NotFound();
        
        var page = mapper.Map<Page>(pageDto);
        page.Id = pageId;
        page.CreatedAt = existingPage.CreatedAt;
        
        var validationResult = await ValidatePage(page);
        if (!validationResult.Success) return validationResult;
        
        var updatedPage = await wikiPageRepository.Update(wikiId, page);
        
        return updatedPage is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(updatedPage))
            : Result<PageDto>.Fail("Failed to update page.");
    }

    public async Task<IResult<PageDto?>> Delete(Ulid wikiId, Ulid pageId, string userId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, userId);
        if (existingWiki is null) return Result<PageDto>.NotFound();
        
        var member = existingWiki.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null ||
            (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.DeleteWikiPages)))
        {
            throw new ForbiddenException("You do not have permission to delete pages for this wiki.");
        }
        
        var existingPage = await wikiPageRepository.GetById(wikiId, pageId, userId);
        if (existingPage is null) return Result<PageDto>.NotFound();
        
        var success = await wikiPageRepository.Delete(wikiId, pageId);
        
        return success
            ? new Result<PageDto>(mapper.Map<PageDto>(existingPage))
            : Result<PageDto>.Fail("Failed to delete page.");
    }
    
    private async Task<IResult<PageDto?>> ValidatePage(Page page)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(page.Title))
        {
            errors.Add("title", ["Title is required"]);
        }
        else
        {
            if (page.Title.Length < 3)
            {
                errors.Add("title", ["Title must be at least 3 characters long"]);
            }
            else if (page.Title.Length > 32)
            {
                errors.Add("title", ["Title must be at most 64 characters long"]);
            }
            else if (!TitleRegex().IsMatch(page.Title))
            {
                errors.Add("title", ["Title contains invalid characters"]);
            }
        }

        if (string.IsNullOrWhiteSpace(page.Slug))
        {
            errors.Add("slug", ["Slug is required"]);
        }
        else
        {
            if (page.Slug.Length < 3)
            {
                errors.Add("slug", ["Slug must be at least 3 characters long"]);
            }
            else if (page.Slug.Length > 32)
            {
                errors.Add("slug", ["Slug must be at most 64 characters long"]);
            }
            else if (!SlugRegex().IsMatch(page.Slug))
            {
                errors.Add("slug", ["Slug may only contain lowercase letters, numbers, and hyphens"]);
            }
            else
            {
                var existingProject =
                    await wikiPageRepository.GetBySlug(page.WikiId, page.Slug, force: true);

                if (existingProject is not null && existingProject.Id != page.Id)
                {
                    errors.Add("slug", ["A page with this slug already exists"]);
                }
            }
        }

        return errors.Count > 0
            ? Result<PageDto?>.ValidationFail(errors)
            : new Result<PageDto?>(null);
    }

    [GeneratedRegex(@"^[\w\s\.,!?'""()&+\-*/\\:;@%<>=|{}\[\]^~]{3,64}$")]
    public static partial Regex TitleRegex();

    [GeneratedRegex(@"^[a-z0-9-]{3,64}$")]
    public static partial Regex SlugRegex();
}