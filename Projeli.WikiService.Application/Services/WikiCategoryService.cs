using System.Text.RegularExpressions;
using AutoMapper;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Models.Events.Categories;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public partial class WikiCategoryService(
    IWikiCategoryRepository wikiCategoryRepository,
    IWikiRepository wikiRepository,
    IWikiEventRepository wikiEventRepository,
    IWikiPageRepository wikiPageRepository,
    IMapper mapper) : IWikiCategoryService
{
    public async Task<IResult<List<CategoryDto>>> GetByWikiId(Ulid wikiId, string? userId)
    {
        var categories = await wikiCategoryRepository.GetByWikiId(wikiId, userId);
        return new Result<List<CategoryDto>>(mapper.Map<List<CategoryDto>>(categories));
    }

    public async Task<IResult<List<CategoryDto>>> GetByProjectId(Ulid projectId, string? userId)
    {
        var categories = await wikiCategoryRepository.GetByProjectId(projectId, userId);
        return new Result<List<CategoryDto>>(mapper.Map<List<CategoryDto>>(categories));
    }

    public async Task<IResult<List<CategoryDto>>> GetByProjectSlug(string wikiId, string? userId)
    {
        var categories = await wikiCategoryRepository.GetByProjectSlug(wikiId, userId);
        return new Result<List<CategoryDto>>(mapper.Map<List<CategoryDto>>(categories));
    }

    public async Task<IResult<CategoryDto?>> GetById(Ulid wikiId, Ulid categoryId, string? userId)
    {
        var category = await wikiCategoryRepository.GetById(wikiId, categoryId, userId);
        return category is not null 
            ? new Result<CategoryDto?>(mapper.Map<CategoryDto>(category))
            : Result<CategoryDto>.NotFound();
    }

    public async Task<IResult<CategoryDto?>> GetBySlug(Ulid wikiId, string categorySlug, string? userId)
    {
        var category = await wikiCategoryRepository.GetBySlug(wikiId, categorySlug, userId);
        return category is not null
            ? new Result<CategoryDto?>(mapper.Map<CategoryDto>(category))
            : Result<CategoryDto?>.NotFound();
    }

    public async Task<IResult<CategoryDto?>> GetByProjectIdAndId(Ulid projectId, Ulid categoryId, string? userId)
    {
        var category = await wikiCategoryRepository.GetByProjectIdAndId(projectId, categoryId, userId);
        return category is not null
            ? new Result<CategoryDto?>(mapper.Map<CategoryDto>(category))
            : Result<CategoryDto?>.NotFound();
    }

    public async Task<IResult<CategoryDto?>> GetByProjectIdAndSlug(Ulid projectId, string categorySlug, string? userId)
    {
        var category = await wikiCategoryRepository.GetByProjectIdAndSlug(projectId, categorySlug, userId);
        return category is not null
            ? new Result<CategoryDto?>(mapper.Map<CategoryDto>(category))
            : Result<CategoryDto?>.NotFound();
    }

    public async Task<IResult<CategoryDto?>> GetByProjectSlugAndId(string projectSlug, Ulid categoryId, string? userId)
    {
        var category = await wikiCategoryRepository.GetByProjectSlugAndId(projectSlug, categoryId, userId);
        return category is not null
            ? new Result<CategoryDto?>(mapper.Map<CategoryDto>(category))
            : Result<CategoryDto?>.NotFound();
    }

    public async Task<IResult<CategoryDto?>> GetByProjectSlugAndSlug(string projectSlug, string categorySlug, string? userId)
    {
        var category = await wikiCategoryRepository.GetByProjectSlugAndSlug(projectSlug, categorySlug, userId);
        return category is not null
            ? new Result<CategoryDto?>(mapper.Map<CategoryDto>(category))
            : Result<CategoryDto?>.NotFound();
    }

    public async Task<IResult<CategoryDto?>> Create(Ulid wikiId, CategoryDto categoryDto, string userId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, userId);
        if (existingWiki is null) return Result<CategoryDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null ||
            (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.CreateWikiCategories)))
        {
            throw new ForbiddenException("You do not have permission to create categories for this wiki.");
        }

        var existingCategories = await wikiCategoryRepository.GetByWikiId(wikiId, userId);
        if (existingCategories.Count >= 128)
            return Result<CategoryDto>.Fail("You have reached the maximum number of categories for this wiki.");

        var category = mapper.Map<Category>(categoryDto);

        category.Id = Ulid.NewUlid();
        category.CreatedAt = DateTime.UtcNow;

        var validationResult = await ValidateCategory(category);
        if (!validationResult.Success) return validationResult;

        var newCategory = await wikiCategoryRepository.Create(wikiId, category);

        if (newCategory is not null)
        {
            await wikiEventRepository.StoreEvent(wikiId, new WikiCategoryCreatedEvent
            {
                UserId = userId,
                CategoryId = newCategory.Id,
                Name = newCategory.Name,
                Slug = newCategory.Slug,
                Description = newCategory.Description,
            });
        }

        return newCategory is not null
            ? new Result<CategoryDto>(mapper.Map<CategoryDto>(newCategory))
            : Result<CategoryDto>.Fail("Failed to create category.");
    }

    public async Task<IResult<CategoryDto?>> Update(Ulid wikiId, Ulid categoryId, CategoryDto categoryDto,
        string userId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, userId);
        if (existingWiki is null) return Result<CategoryDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null ||
            (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWikiCategories)))
        {
            throw new ForbiddenException("You do not have permission to edit categories for this wiki.");
        }

        var existingCategory = await wikiCategoryRepository.GetById(wikiId, categoryId, userId);
        if (existingCategory is null) return Result<CategoryDto>.NotFound();
        
        if (existingCategory.Name == categoryDto.Name &&
            existingCategory.Slug == categoryDto.Slug &&
            existingCategory.Description == categoryDto.Description)
        {
            return new Result<CategoryDto>(mapper.Map<CategoryDto>(existingCategory));
        }
        
        existingCategory.UpdatedAt = DateTime.UtcNow;
        existingCategory.Name = categoryDto.Name;
        existingCategory.Slug = categoryDto.Slug;
        existingCategory.Description = categoryDto.Description;

        var validationResult = await ValidateCategory(existingCategory);
        if (!validationResult.Success) return validationResult;

        var updatedCategory = await wikiCategoryRepository.Update(wikiId, existingCategory);

        if (updatedCategory is not null)
        {
            await wikiEventRepository.StoreEvent(wikiId, new WikiCategoryUpdatedEvent
            {
                UserId = userId,
                CategoryId = updatedCategory.Id,
                Name = updatedCategory.Name,
                Slug = updatedCategory.Slug,
                Description = updatedCategory.Description,
            });
        }
        
        return updatedCategory is not null
            ? new Result<CategoryDto>(mapper.Map<CategoryDto>(updatedCategory))
            : Result<CategoryDto>.Fail("Failed to update category.");
    }

    public async Task<IResult<CategoryDto?>> UpdatePages(Ulid wikiId, Ulid categoryId, List<Ulid> pageIds, string userId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, userId);
        if (existingWiki is null) return Result<CategoryDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null ||
            (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWikiPages)))
        {
            throw new ForbiddenException("You do not have permission to update pages for this wiki.");
        }

        var existingCategory = await wikiCategoryRepository.GetById(wikiId, categoryId, userId);
        if (existingCategory is null) return Result<CategoryDto>.NotFound();

        var pages = await wikiPageRepository.GetByWikiId(wikiId, userId);
        if (pages.Count == 0)
        {
            return Result<CategoryDto>.Fail("No categories found for this wiki.");
        }

        if (pageIds.Count == existingCategory.Pages.Count && pageIds.All(id => existingCategory.Pages.Any(c => c.Id == id)))
        {
            return new Result<CategoryDto?>(mapper.Map<CategoryDto>(existingCategory), "No changes made.");
        }

        existingCategory.UpdatedAt = DateTime.UtcNow;

        var updatedCategory = await wikiCategoryRepository.UpdatePages(wikiId, existingCategory, pageIds);

        if (updatedCategory is not null)
        {
            await wikiEventRepository.StoreEvent(wikiId, new WikiCategoryUpdatedPagesEvent
            {
                UserId = userId,
                CategoryId = updatedCategory.Id,
                Pages = updatedCategory.Pages.Select(page => new WikiCategoryUpdatedPagesEvent.SimplePage
                {
                    Id = page.Id,
                    Title = page.Title,
                    Slug = page.Slug
                }).ToList()
            });
        }

        return updatedCategory is not null
            ? new Result<CategoryDto>(mapper.Map<CategoryDto>(updatedCategory))
            : Result<CategoryDto>.Fail("Failed to update category.");
    }

    public async Task<IResult<CategoryDto?>> Delete(Ulid wikiId, Ulid categoryId, string userId)
    {
        var existingWiki = await wikiRepository.GetById(wikiId, userId);
        if (existingWiki is null) return Result<CategoryDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null ||
            (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.DeleteWikiCategories)))
        {
            throw new ForbiddenException("You do not have permission to delete categories for this wiki.");
        }

        var existingCategory = await wikiCategoryRepository.GetById(wikiId, categoryId, userId);
        if (existingCategory is null) return Result<CategoryDto>.NotFound();

        var success = await wikiCategoryRepository.Delete(wikiId, categoryId);

        if (success)
        {
            await wikiEventRepository.StoreEvent(wikiId, new WikiCategoryDeletedEvent
            {
                UserId = userId,
                CategoryId = existingCategory.Id,
            });
        }
        
        return success
            ? new Result<CategoryDto>(mapper.Map<CategoryDto>(existingCategory))
            : Result<CategoryDto>.Fail("Failed to delete category.");
    }

    private async Task<IResult<CategoryDto?>> ValidateCategory(Category category)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(category.Name))
        {
            errors.Add("name", ["Name is required"]);
        }
        else
        {
            if (category.Name.Length < 3)
            {
                errors.Add("name", ["Name must be at least 3 characters long"]);
            }
            else if (category.Name.Length > 32)
            {
                errors.Add("name", ["Name must be at most 32 characters long"]);
            }
            else if (!NameRegex().IsMatch(category.Name))
            {
                errors.Add("name", ["Name contains invalid characters"]);
            }
        }

        if (string.IsNullOrWhiteSpace(category.Slug))
        {
            errors.Add("slug", ["Slug is required"]);
        }
        else
        {
            if (category.Slug.Length < 3)
            {
                errors.Add("slug", ["Slug must be at least 3 characters long"]);
            }
            else if (category.Slug.Length > 32)
            {
                errors.Add("slug", ["Slug must be at most 32 characters long"]);
            }
            else if (!SlugRegex().IsMatch(category.Slug))
            {
                errors.Add("slug", ["Slug may only contain lowercase letters, numbers, and hyphens"]);
            }
            else
            {
                var existingProject =
                    await wikiCategoryRepository.GetBySlug(category.WikiId, category.Slug, force: true);

                if (existingProject is not null && existingProject.Id != category.Id)
                {
                    errors.Add("slug", ["A category with this slug already exists"]);
                }
            }
        }

        return errors.Count > 0
            ? Result<CategoryDto?>.ValidationFail(errors)
            : new Result<CategoryDto?>(null);
    }

    [GeneratedRegex(@"^[\w\s\.,!?'""()&+\-*/\\:;@%<>=|{}\[\]^~]{3,32}$")]
    public static partial Regex NameRegex();

    [GeneratedRegex(@"^[a-z0-9-]{3,32}$")]
    public static partial Regex SlugRegex();
}