using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Domain.Results;
using Projeli.Shared.Infrastructure.Extensions;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Requests;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Api.Controllers.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/wikis/{wikiId}/categories")]
public class WikiCategoryController(IWikiCategoryService wikiCategoryService, IMapper mapper) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromRoute] Ulid wikiId)
    {
        var result = await wikiCategoryService.GetByWikiId(wikiId, User.TryGetId());
        return HandleResult(result.Success
            ? new Result<List<SimpleCategoryResponse>>(mapper.Map<List<SimpleCategoryResponse>>(result.Data))
            : result);
    }
    
    [HttpGet("project")]
    public async Task<IActionResult> GetCategoriesByProject([FromRoute] string wikiId)
    {
        IResult<List<CategoryDto>> result;
        if (Ulid.TryParse(wikiId, out var projectId))
        {
            result = await wikiCategoryService.GetByProjectId(projectId, User.TryGetId());
        } 
        else
        {
            result = await wikiCategoryService.GetByProjectSlug(wikiId, User.TryGetId());
        }
        
        return HandleResult(result.Success
            ? new Result<List<SimpleCategoryResponse>>(mapper.Map<List<SimpleCategoryResponse>>(result.Data))
            : result);
    }
    
    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategory([FromRoute] Ulid wikiId, [FromRoute] string categoryId)
    {
        IResult<CategoryDto?> result;
        if (Ulid.TryParse(categoryId, out var ulid))
        {
            result = await wikiCategoryService.GetById(wikiId, ulid, User.TryGetId());
        }
        else
        {
            result = await wikiCategoryService.GetBySlug(wikiId, categoryId, User.TryGetId());
        }

        return HandleResult(result.Success
            ? new Result<CategoryResponse>(mapper.Map<CategoryResponse>(result.Data))
            : result);
    }
    
    [HttpGet("{categoryId}/project")]
    public async Task<IActionResult> GetCategoryByProject([FromRoute] string wikiId, [FromRoute] string categoryId)
    {
        IResult<CategoryDto?> result;
        if (Ulid.TryParse(wikiId, out var projectId))
        {
            if (Ulid.TryParse(categoryId, out var categoryIdUlid))
            {
                result = await wikiCategoryService.GetByProjectIdAndId(projectId, categoryIdUlid, User.TryGetId());
            }
            else
            {
                result = await wikiCategoryService.GetByProjectIdAndSlug(projectId, categoryId, User.TryGetId());
            }
        }
        else
        {
            if (Ulid.TryParse(categoryId, out var categoryIdUlid))
            {
                result = await wikiCategoryService.GetByProjectSlugAndId(wikiId, categoryIdUlid, User.TryGetId());
            }
            else
            {
                result = await wikiCategoryService.GetByProjectSlugAndSlug(wikiId, categoryId, User.TryGetId());
            }
        }

        return HandleResult(result.Success
            ? new Result<CategoryResponse>(mapper.Map<CategoryResponse>(result.Data))
            : result);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategory([FromRoute] Ulid wikiId, [FromBody] CreateCategoryRequest request)
    {
        var categoryDto = mapper.Map<CategoryDto>(request);
        var result = await wikiCategoryService.Create(wikiId, categoryDto, User.GetId());
        return HandleResult(result.Success
            ? new Result<SimpleCategoryResponse>(mapper.Map<SimpleCategoryResponse>(result.Data))
            : result);
    }
    
    [HttpPut("{categoryId}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory([FromRoute] Ulid wikiId, [FromRoute] Ulid categoryId, [FromBody] UpdateCategoryRequest request)
    {
        var categoryDto = mapper.Map<CategoryDto>(request);
        var result = await wikiCategoryService.Update(wikiId, categoryId, categoryDto, User.GetId());
        return HandleResult(result.Success
            ? new Result<SimpleCategoryResponse>(mapper.Map<SimpleCategoryResponse>(result.Data))
            : result);
    }
    
    [HttpPut("{pageId}/pages")]
    [Authorize]
    public async Task<IActionResult> UpdateCategoryPages([FromRoute] Ulid wikiId, [FromRoute] Ulid pageId,
        [FromBody] UpdateCategoryPagesRequest request)
    {
        var result = await wikiCategoryService.UpdatePages(wikiId, pageId, request.PageIds, User.GetId());
        return HandleResult(result.Success
            ? new Result<CategoryResponse>(mapper.Map<CategoryResponse>(result.Data), result.Message)
            : result);
    }
    
    [HttpDelete("{categoryId}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory([FromRoute] Ulid wikiId, [FromRoute] Ulid categoryId)
    {
        var result = await wikiCategoryService.Delete(wikiId, categoryId, User.GetId());
        return HandleResult(result.Success
            ? new Result<SimpleCategoryResponse>(mapper.Map<SimpleCategoryResponse>(result.Data))
            : result);
    }
}