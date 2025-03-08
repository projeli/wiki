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