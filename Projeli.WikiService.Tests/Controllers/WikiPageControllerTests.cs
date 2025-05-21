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
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Tests.Controllers;

public class WikiPageControllerTests
{
    private readonly Mock<IWikiPageService> _wikiPageServiceMock;
    private readonly WikiPageController _controller;

    public WikiPageControllerTests()
    {
        _wikiPageServiceMock = new Mock<IWikiPageService>();
        var mapper =
            new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(WikiProfile)))).CreateMapper();
        _controller = new WikiPageController(_wikiPageServiceMock.Object, mapper);
    }

    [Fact]
    public async Task GetPagesByWikiId_ReturnsOkResult_Always()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var pagesResult = new List<PageDto>
        {
            new() { Id = Ulid.NewUlid(), Title = "Test Page" }
        };
        _wikiPageServiceMock.Setup(s => s.GetByWikiId(wikiId, null, false))
            .ReturnsAsync(new Result<List<PageDto>>(pagesResult));

        // Act
        var result = await _controller.GetPages(wikiId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<List<SimplePageResponse>>>(okResult.Value);
        Assert.Single(returnValue.Data!);
    }

    [Fact]
    public async Task GetPagesByProjectId_ReturnsOkResult_Always()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var pagesResult = new List<PageDto>
        {
            new() { Id = Ulid.NewUlid(), Title = "Test Page" }
        };
        _wikiPageServiceMock.Setup(s => s.GetByProjectId(projectId, null))
            .ReturnsAsync(new Result<List<PageDto>>(pagesResult));

        // Act
        var result = await _controller.GetPagesByProject(projectId.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<List<SimplePageResponse>>>(okResult.Value);
        Assert.Single(returnValue.Data!);
    }

    [Fact]
    public async Task GetPagesByProjectSlug_ReturnsOkResult_Always()
    {
        // Arrange
        var projectSlug = "test-project";
        var pagesResult = new List<PageDto>
        {
            new() { Id = Ulid.NewUlid(), Title = "Test Page" }
        };
        _wikiPageServiceMock.Setup(s => s.GetByProjectSlug(projectSlug, null))
            .ReturnsAsync(new Result<List<PageDto>>(pagesResult));

        // Act
        var result = await _controller.GetPagesByProject(projectSlug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<List<SimplePageResponse>>>(okResult.Value);
        Assert.Single(returnValue.Data!);
    }

    [Fact]
    public async Task GetPageById_ReturnsOkResult_WhenPageExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var pageId = Ulid.NewUlid();
        var pageResult = new Result<PageDto?>(new PageDto { Id = pageId, Title = "Test Page" });
        _wikiPageServiceMock.Setup(s => s.GetById(wikiId, pageId, null))
            .ReturnsAsync(pageResult);

        // Act
        var result = await _controller.GetPage(wikiId, pageId.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageId, returnValue.Data!.Id);
    }

    [Fact]
    public async Task GetPageBySlug_ReturnsOkResult_WhenPageExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string pageSlug = "test-page";
        var pageResult = new Result<PageDto?>(new PageDto { Slug = pageSlug, Title = "Test Page" });
        _wikiPageServiceMock.Setup(s => s.GetBySlug(wikiId, pageSlug, null))
            .ReturnsAsync(pageResult);

        // Act
        var result = await _controller.GetPage(wikiId, pageSlug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageSlug, returnValue.Data!.Slug);
    }

    [Fact]
    public async Task GetPageByIdAndProjectId_ReturnsOkResult_WhenPageExists()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var pageId = Ulid.NewUlid();
        var pageResult = new Result<PageDto?>(new PageDto { Id = pageId, Title = "Test Page" });
        _wikiPageServiceMock.Setup(s => s.GetByProjectIdAndId(projectId, pageId, null))
            .ReturnsAsync(pageResult);

        // Act
        var result = await _controller.GetPageByProject(projectId.ToString(), pageId.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageId, returnValue.Data!.Id);
    }

    [Fact]
    public async Task GetPageByIdAndProjectSlug_ReturnsOkResult_WhenPageExists()
    {
        // Arrange
        const string projectSlug = "test-project";
        var pageId = Ulid.NewUlid();
        var pageResult = new Result<PageDto?>(new PageDto { Id = pageId, Title = "Test Page" });
        _wikiPageServiceMock.Setup(s => s.GetByProjectSlugAndId(projectSlug, pageId, null))
            .ReturnsAsync(pageResult);

        // Act
        var result = await _controller.GetPageByProject(projectSlug, pageId.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageId, returnValue.Data!.Id);
    }

    [Fact]
    public async Task GetPageBySlugAndProjectId_ReturnsOkResult_WhenPageExists()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        const string pageSlug = "test-page";
        var pageResult = new Result<PageDto?>(new PageDto { Slug = pageSlug, Title = "Test Page" });
        _wikiPageServiceMock.Setup(s => s.GetByProjectIdAndSlug(projectId, pageSlug, null))
            .ReturnsAsync(pageResult);

        // Act
        var result = await _controller.GetPageByProject(projectId.ToString(), pageSlug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageSlug, returnValue.Data!.Slug);
    }

    [Fact]
    public async Task GetPageBySlugAndProjectSlug_ReturnsOkResult_WhenPageExists()
    {
        // Arrange
        const string projectSlug = "test-project";
        const string pageSlug = "test-page";
        var pageResult = new Result<PageDto?>(new PageDto { Slug = pageSlug, Title = "Test Page" });
        _wikiPageServiceMock.Setup(s => s.GetByProjectSlugAndSlug(projectSlug, pageSlug, null))
            .ReturnsAsync(pageResult);

        // Act
        var result = await _controller.GetPageByProject(projectSlug, pageSlug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageSlug, returnValue.Data!.Slug);
    }

    [Fact]
    public async Task CreatePage_ReturnsOkResult_WhenPageCreated()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var pageDto = new PageDto { Title = "Test Page", Slug = "test-page" };
        var pageResult = new Result<PageDto?>(new PageDto { Id = Ulid.NewUlid(), Title = "Test Page" });
        _wikiPageServiceMock.Setup(s => s.Create(wikiId, It.IsAny<PageDto>(), "user123"))
            .ReturnsAsync(pageResult);
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
        var result = await _controller.CreatePage(wikiId,
            new CreatePageRequest { Title = pageDto.Title, Slug = pageDto.Slug });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageDto.Title, returnValue.Data!.Title);
    }

    [Fact]
    public async Task UpdatePage_ReturnsOkResult_WhenPageUpdated()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var pageId = Ulid.NewUlid();
        var pageDto = new PageDto { Title = "Test Page", Slug = "test-page" };
        var pageResult = new Result<PageDto?>(new PageDto { Id = pageId, Title = "Test Page", Slug = "test-page" });
        _wikiPageServiceMock.Setup(s => s.Update(wikiId, pageId, It.IsAny<PageDto>(), "user123"))
            .ReturnsAsync(pageResult);
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
        var result = await _controller.UpdatePage(wikiId, pageId,
            new UpdatePageRequest { Title = pageDto.Title, Slug = pageDto.Slug });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageDto.Title, returnValue.Data!.Title);
    }

    [Fact]
    public async Task UpdatePageContent_ReturnsOkResult_WhenPageContentUpdated()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var pageId = Ulid.NewUlid();
        const string content = "Test Content";
        var pageResult = new Result<PageDto?>(new PageDto { Id = pageId, Content = content });
        _wikiPageServiceMock.Setup(s => s.UpdateContent(wikiId, pageId, content, "user123"))
            .ReturnsAsync(pageResult);
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
            await _controller.UpdatePageContent(wikiId, pageId, new UpdatePageContentRequest { Content = content });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(content, returnValue.Data!.Content);
    }

    [Fact]
    public async Task UpdatePageCategories_ReturnsOkResult_WhenPageCategoriesUpdated()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var pageId = Ulid.NewUlid();
        var categoryIds = new List<Ulid> { Ulid.NewUlid() };
        var pageResult = new Result<PageDto?>(new PageDto
            { Id = pageId, Categories = categoryIds.Select(id => new CategoryDto { Id = id }).ToList() });
        _wikiPageServiceMock.Setup(s => s.UpdateCategories(wikiId, pageId, categoryIds, "user123"))
            .ReturnsAsync(pageResult);
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
        var result = await _controller.UpdatePageCategories(wikiId, pageId,
            new UpdatePageCategoriesRequest { CategoryIds = categoryIds });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Single(returnValue.Data!.Categories);
    }
    
    [Fact]
    public async Task UpdatePageStatus_ReturnsOkResult_WhenPageStatusUpdated()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var pageId = Ulid.NewUlid();
        const PageStatus status = PageStatus.Published;
        var pageResult = new Result<PageDto?>(new PageDto { Id = pageId, Status = status });
        _wikiPageServiceMock.Setup(s => s.UpdateStatus(wikiId, pageId, status, "user123"))
            .ReturnsAsync(pageResult);
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
            await _controller.UpdatePageStatus(wikiId, pageId, new UpdatePageStatusRequest { Status = status });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(status, returnValue.Data!.Status);
    }
    
    [Fact]
    public async Task DeletePage_ReturnsOkResult_WhenPageDeleted()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var pageId = Ulid.NewUlid();
        var pageResult = new Result<PageDto?>(new PageDto { Id = pageId });
        _wikiPageServiceMock.Setup(s => s.Delete(wikiId, pageId, "user123"))
            .ReturnsAsync(pageResult);
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
        var result = await _controller.DeletePage(wikiId, pageId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<PageResponse>>(okResult.Value);
        Assert.Equal(pageId, returnValue.Data!.Id);
    }
}