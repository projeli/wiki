using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Api.Controllers.V1;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Requests;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Application.Profiles;
using Projeli.WikiService.Application.Services.Interfaces;

namespace Projeli.WikiService.Tests.Controllers;

public class WikiCategoryControllerTests
{
    private readonly Mock<IWikiCategoryService> _wikiCategoryServiceMock;
    private readonly WikiCategoryController _controller;

    public WikiCategoryControllerTests()
    {
        _wikiCategoryServiceMock = new Mock<IWikiCategoryService>();
        var mapper =
            new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(WikiProfile)))).CreateMapper();
        _controller = new WikiCategoryController(_wikiCategoryServiceMock.Object, mapper);
    }
    
    [Fact]
    public async Task GetCategories_ReturnsOkResult_WhenCategoriesExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var categoriesResult = new List<CategoryDto>
        {
            new() {Id = Ulid.NewUlid(), Name = "Test Category"}
        };
        _wikiCategoryServiceMock.Setup(s => s.GetByWikiId(wikiId, null))
            .ReturnsAsync(new Result<List<CategoryDto>>(categoriesResult));

        // Act
        var result = await _controller.GetCategories(wikiId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<List<SimpleCategoryResponse>>>(okResult.Value);
        Assert.Single(returnValue.Data!);
    }
    
    [Fact]
    public async Task GetCategoriesByProjectId_ReturnsOkResult_WhenCategoriesExist()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var categoriesResult = new List<CategoryDto>
        {
            new() {Id = Ulid.NewUlid(), Name = "Test Category"}
        };
        _wikiCategoryServiceMock.Setup(s => s.GetByProjectId(projectId, null))
            .ReturnsAsync(new Result<List<CategoryDto>>(categoriesResult));

        // Act
        var result = await _controller.GetCategoriesByProject(projectId.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<List<SimpleCategoryResponse>>>(okResult.Value);
        Assert.Single(returnValue.Data!);
    }
    
    [Fact]
    public async Task GetCategoriesByProjectSlug_ReturnsOkResult_WhenCategoriesExist()
    {
        // Arrange
        const string projectSlug = "test-project";
        var categoriesResult = new List<CategoryDto>
        {
            new() {Id = Ulid.NewUlid(), Name = "Test Category"}
        };
        _wikiCategoryServiceMock.Setup(s => s.GetByProjectSlug(projectSlug, null))
            .ReturnsAsync(new Result<List<CategoryDto>>(categoriesResult));

        // Act
        var result = await _controller.GetCategoriesByProject(projectSlug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<List<SimpleCategoryResponse>>>(okResult.Value);
        Assert.Single(returnValue.Data!);
    }
    
    [Fact]
    public async Task CreateCategory_ReturnsOkResult_WhenCategoryCreated()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var categoryDto = new CategoryDto {Name = "Test Category"};
        var categoryResponse = new SimpleCategoryResponse {Name = categoryDto.Name};
        _wikiCategoryServiceMock.Setup(s => s.Create(wikiId, It.IsAny<CategoryDto>(), "user123"))
            .ReturnsAsync(new Result<CategoryDto>(categoryDto));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                ]))
            }
        };
        
        // Act
        var result = await _controller.CreateCategory(wikiId, new CreateCategoryRequest {Name = categoryDto.Name});

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<SimpleCategoryResponse>>(okResult.Value);
        Assert.Equal(categoryResponse.Name, returnValue.Data!.Name);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsOkResult_WhenCategoryUpdated()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var categoryId = Ulid.NewUlid();
        var categoryDto = new CategoryDto { Name = "Test Category" };
        var categoryResponse = new SimpleCategoryResponse { Name = categoryDto.Name };
        _wikiCategoryServiceMock.Setup(s => s.Update(wikiId, categoryId, It.IsAny<CategoryDto>(), "user123"))
            .ReturnsAsync(new Result<CategoryDto>(categoryDto));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                ]))
            }
        };
        
        // Act
        var result =
            await _controller.UpdateCategory(wikiId, categoryId, new UpdateCategoryRequest { Name = categoryDto.Name });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<SimpleCategoryResponse>>(okResult.Value);
        Assert.Equal(categoryResponse.Name, returnValue.Data!.Name);
    }
    
    [Fact]
    public async Task DeleteCategory_ReturnsOkResult_WhenCategoryDeleted()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var categoryId = Ulid.NewUlid();
        var categoryDto = new CategoryDto { Name = "Test Category" };
        var categoryResponse = new SimpleCategoryResponse { Name = categoryDto.Name };
        _wikiCategoryServiceMock.Setup(s => s.Delete(wikiId, categoryId, "user123"))
            .ReturnsAsync(new Result<CategoryDto>(categoryDto));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, "user123")
                ]))
            }
        };
        
        // Act
        var result = await _controller.DeleteCategory(wikiId, categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<SimpleCategoryResponse>>(okResult.Value);
        Assert.Equal(categoryResponse.Name, returnValue.Data!.Name);
    }
}