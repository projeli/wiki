using System.Text.RegularExpressions;
using AutoMapper;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Models.Events.Pages;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public partial class WikiPageService(
    IWikiPageRepository wikiPageRepository,
    IWikiRepository wikiRepository,
    IWikiCategoryRepository wikiCategoryRepository,
    IEventRepository eventRepository,
    IMapper mapper) : IWikiPageService
{
    public async Task<IResult<List<PageDto>>> GetByWikiId(Ulid wikiId, string? userId = null, bool force = false)
    {
        var pages = await wikiPageRepository.GetByWikiId(wikiId, userId, force);
        return new Result<List<PageDto>>(mapper.Map<List<PageDto>>(pages));
    }

    public async Task<IResult<List<PageDto>>> GetByProjectId(Ulid projectId, string? userId)
    {
        var pages = await wikiPageRepository.GetByProjectId(projectId, userId);
        return new Result<List<PageDto>>(mapper.Map<List<PageDto>>(pages));
    }

    public async Task<IResult<List<PageDto>>> GetByProjectSlug(string wikiId, string? userId)
    {
        var pages = await wikiPageRepository.GetByProjectSlug(wikiId, userId);
        return new Result<List<PageDto>>(mapper.Map<List<PageDto>>(pages));
    }

    public async Task<IResult<PageDto?>> GetById(Ulid wikiId, Ulid pageId, string? userId)
    {
        var page = await wikiPageRepository.GetById(wikiId, pageId, userId);
        return page is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(page))
            : Result<PageDto>.NotFound();
    }

    public async Task<IResult<PageDto?>> GetBySlug(Ulid wikiId, string pageSlug, string? userId)
    {
        var page = await wikiPageRepository.GetBySlug(wikiId, pageSlug, userId);
        return page is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(page))
            : Result<PageDto>.NotFound();
    }

    public async Task<IResult<PageDto?>> GetByProjectIdAndId(Ulid projectId, Ulid pageId, string? userId)
    {
        var page = await wikiPageRepository.GetByProjectIdAndId(projectId, pageId, userId);
        return page is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(page))
            : Result<PageDto>.NotFound();
    }

    public async Task<IResult<PageDto?>> GetByProjectIdAndSlug(Ulid projectId, string pageSlug, string? userId)
    {
        var page = await wikiPageRepository.GetByProjectIdAndSlug(projectId, pageSlug, userId);
        return page is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(page))
            : Result<PageDto>.NotFound();
    }

    public async Task<IResult<PageDto?>> GetByProjectSlugAndId(string projectSlug, Ulid pageId, string? userId)
    {
        var page = await wikiPageRepository.GetByProjectSlugAndId(projectSlug, pageId, userId);
        return page is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(page))
            : Result<PageDto>.NotFound();
    }

    public async Task<IResult<PageDto?>> GetByProjectSlugAndSlug(string projectSlug, string pageSlug, string? userId)
    {
        var page = await wikiPageRepository.GetByProjectSlugAndSlug(projectSlug, pageSlug, userId);
        return page is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(page))
            : Result<PageDto>.NotFound();
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

        if (newPage is not null)
        {
            await eventRepository.StoreEvent(wikiId, new WikiPageCreatedEvent
            {
                UserId = userId,
                WikiPageId = newPage.Id,
                Title = newPage.Title,
                Slug = newPage.Slug
            });
        }

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

        if (existingPage.Title == pageDto.Title && existingPage.Slug == pageDto.Slug)
        {
            return new Result<PageDto?>(mapper.Map<PageDto>(existingPage), "No changes made.");
        }
        
        existingPage.Title = pageDto.Title;
        existingPage.Slug = pageDto.Slug;

        var validationResult = await ValidatePage(existingPage);
        if (!validationResult.Success) return validationResult;

        var updatedPage = await wikiPageRepository.Update(wikiId, existingPage);

        if (updatedPage is not null)
        {
            await eventRepository.StoreEvent(wikiId, new WikiPageUpdatedDetailsEvent
            {
                UserId = userId,
                WikiPageId = updatedPage.Id,
                Title = updatedPage.Title,
                Slug = updatedPage.Slug
            });
        }

        return updatedPage is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(updatedPage))
            : Result<PageDto>.Fail("Failed to update page.");
    }

    public async Task<IResult<PageDto?>> UpdateContent(Ulid wikiId, Ulid pageId, string content, string userId)
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

        if (existingPage.Content == content)
        {
            return new Result<PageDto?>(mapper.Map<PageDto>(existingPage), "No changes made.");
        }
        
        existingPage.Content = content;

        var updatedPage = await wikiPageRepository.UpdateContent(wikiId, existingPage);

        if (updatedPage is not null)
        {
            await eventRepository.StoreEvent(wikiId, new WikiPageUpdatedContentEvent
            {
                UserId = userId,
                WikiPageId = updatedPage.Id,
                Content = updatedPage.Content
            });
        }

        return updatedPage is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(updatedPage))
            : Result<PageDto>.Fail("Failed to update page.");
    }

    public async Task<IResult<PageDto?>> UpdateCategories(Ulid wikiId, Ulid pageId, List<Ulid> categoryIds,
        string userId)
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

        var categories = await wikiCategoryRepository.GetByWikiId(wikiId, userId);
        if (categories.Count == 0)
        {
            return Result<PageDto>.Fail("No categories found for this wiki.");
        }

        if (categories.All(c => categoryIds.Contains(c.Id)))
        {
            return new Result<PageDto?>(mapper.Map<PageDto>(existingPage), "No changes made.");
        }

        existingPage.UpdatedAt = DateTime.UtcNow;

        var updatedPage = await wikiPageRepository.UpdateCategories(wikiId, existingPage, categoryIds);

        if (updatedPage is not null)
        {
            await eventRepository.StoreEvent(wikiId, new WikiPageUpdatedCategoriesEvent
            {
                UserId = userId,
                WikiPageId = updatedPage.Id,
                Categories = updatedPage.Categories.Select(category => new WikiPageUpdatedCategoriesEvent.SimpleCategory
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    Description = category.Description
                }).ToList()
            });
        }

        return updatedPage is not null
            ? new Result<PageDto>(mapper.Map<PageDto>(updatedPage))
            : Result<PageDto>.Fail("Failed to update page.");
    }

    public async Task<IResult<PageDto?>> UpdateStatus(Ulid wikiId, Ulid pageId, PageStatus status, string userId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, userId);
        if (existingWiki is null) return Result<PageDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null ||
            (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.PublishWikiPages)))
        {
            throw new ForbiddenException("You do not have permission to update pages for this wiki.");
        }

        var existingPage = await wikiPageRepository.GetById(wikiId, pageId, userId);
        if (existingPage is null) return Result<PageDto>.NotFound();

        switch (status)
        {
            case PageStatus.Draft:
                return Result<PageDto>.Fail("Cannot set status to draft since it is the default status");
            case PageStatus.Published
                when existingPage.Status != PageStatus.Draft && existingPage.Status != PageStatus.Archived:
                return Result<PageDto>.Fail("Cannot set status to published");
            case PageStatus.Archived when existingPage.Status != PageStatus.Published:
                return Result<PageDto>.Fail("Cannot set status to archived");
        }

        var updatedPage = await wikiPageRepository.UpdateStatus(wikiId, pageId, status);

        if (updatedPage is not null)
        {
            await eventRepository.StoreEvent(wikiId, new WikiPageUpdatedStatusEvent
            {
                UserId = userId,
                WikiPageId = updatedPage.Id,
                Status = updatedPage.Status
            });
        }

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

        if (success)
        {
            await eventRepository.StoreEvent(wikiId, new WikiPageDeletedEvent
            {
                UserId = userId,
                WikiPageId = existingPage.Id
            });
        }

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