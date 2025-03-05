using System.Text.RegularExpressions;
using AutoMapper;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public partial class WikiCategoryService(
    IWikiCategoryRepository wikiCategoryRepository,
    IWikiRepository wikiRepository,
    IMapper mapper) : IWikiCategoryService
{
    public async Task<IResult<List<CategoryDto>>> GetByWikiId(Ulid wikiId, string? userId)
    {
        var categories = await wikiCategoryRepository.GetByWikiId(wikiId, userId);
        return new Result<List<CategoryDto>>(mapper.Map<List<CategoryDto>>(categories));
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

        return newCategory is not null
            ? new Result<CategoryDto>(mapper.Map<CategoryDto>(newCategory))
            : Result<CategoryDto>.Fail("Failed to create category.");
    }

    public async Task<IResult<CategoryDto?>> Update(Ulid wikiId, Ulid categoryId, CategoryDto categoryDto, string userId)
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

        var category = mapper.Map<Category>(categoryDto);
        category.Id = existingCategory.Id;
        category.UpdatedAt = DateTime.UtcNow;

        var validationResult = await ValidateCategory(category);
        if (!validationResult.Success) return validationResult;

        var updatedCategory = await wikiCategoryRepository.Update(wikiId, category);

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