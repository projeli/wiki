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

public class WikiControllerTests
{
    private readonly Mock<IWikiService> _wikiServiceMock;
    private readonly WikiController _controller;

    public WikiControllerTests()
    {
        _wikiServiceMock = new Mock<IWikiService>();
        var mapper =
            new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(WikiProfile)))).CreateMapper();
        _controller = new WikiController(_wikiServiceMock.Object, mapper);
    }

    [Fact]
    public async Task GetWikiById_ReturnsOkResult_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var wikiResult = new Result<WikiDto?>(new WikiDto { Id = wikiId });
        _wikiServiceMock.Setup(s => s.GetById(wikiId, null, false)).ReturnsAsync(wikiResult);

        // Act
        var result = await _controller.GetWikiById(wikiId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<WikiResponse>>(okResult.Value);
        Assert.True(returnValue.Success);
        Assert.NotNull(returnValue.Data);
    }

    [Fact]
    public async Task GetWikiByProjectId_ReturnsOkResult_WhenWikiExists()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var wikiResult = new Result<WikiDto?>(new WikiDto { Id = Ulid.NewUlid() });
        _wikiServiceMock.Setup(s => s.GetByProjectId(projectId, null, false)).ReturnsAsync(wikiResult);

        // Act
        var result = await _controller.GetWikiByProjectId(projectId.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<WikiResponse>>(okResult.Value);
        Assert.True(returnValue.Success);
        Assert.NotNull(returnValue.Data);
    }

    [Fact]
    public async Task GetWikiByProjectId_ReturnsOkResult_WhenWikiExistsBySlug()
    {
        // Arrange
        var projectSlug = "test-project";
        var wikiResult = new Result<WikiDto?>(new WikiDto { Id = Ulid.NewUlid() });
        _wikiServiceMock.Setup(s => s.GetByProjectSlug(projectSlug, null, false)).ReturnsAsync(wikiResult);

        // Act
        var result = await _controller.GetWikiByProjectId(projectSlug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<WikiResponse>>(okResult.Value);
        Assert.True(returnValue.Success);
        Assert.NotNull(returnValue.Data);
    }

    [Fact]
    public async Task GetWikiStatistics_ReturnsOkResult_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var wikiStatisticsResult = new Result<WikiStatisticsDto?>(new WikiStatisticsDto { WikiId = wikiId });
        _wikiServiceMock.Setup(s => s.GetStatistics(wikiId, null)).ReturnsAsync(wikiStatisticsResult);

        // Act
        var result = await _controller.GetWikiStatistics(wikiId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<WikiStatisticsResponse>>(okResult.Value);
        Assert.True(returnValue.Success);
        Assert.NotNull(returnValue.Data);
    }

    [Fact]
    public async Task UpdateWikiStatus_ReturnsOkResult_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var updateWikiStatusRequest = new UpdateWikiStatusRequest { Status = WikiStatus.Draft };
        var wikiResult = new Result<WikiDto?>(new WikiDto { Id = wikiId });
        _wikiServiceMock.Setup(s => s.UpdateStatus(wikiId, updateWikiStatusRequest.Status, "user123"))
            .ReturnsAsync(wikiResult);
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
        var result = await _controller.UpdateWikiStatus(wikiId, updateWikiStatusRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<WikiResponse>>(okResult.Value);
        Assert.True(returnValue.Success);
        Assert.NotNull(returnValue.Data);
    }

    [Fact]
    public async Task UpdateWikiContent_ReturnsOkResult_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var updateWikiContentRequest = new UpdateWikiContentRequest { Content = "Test Content" };
        var wikiResult = new Result<WikiDto?>(new WikiDto { Id = wikiId });
        _wikiServiceMock.Setup(s => s.UpdateContent(wikiId, updateWikiContentRequest.Content, "user123"))
            .ReturnsAsync(wikiResult);
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
        var result = await _controller.UpdateWikiContent(wikiId, updateWikiContentRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<WikiResponse>>(okResult.Value);
        Assert.True(returnValue.Success);
        Assert.NotNull(returnValue.Data);
    }

    [Fact]
    public async Task UpdateWikiSidebar_ReturnsOkResult_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var updateWikiSidebarRequest = new UpdateWikiSidebarRequest
            { Sidebar = new WikiConfigDto.WikiConfigSidebarDto() };
        var wikiResult = new Result<WikiDto?>(new WikiDto { Id = wikiId });
        _wikiServiceMock.Setup(s => s.UpdateSidebar(wikiId, updateWikiSidebarRequest.Sidebar, "user123"))
            .ReturnsAsync(wikiResult);
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
        var result = await _controller.UpdateWikiSidebar(wikiId, updateWikiSidebarRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<WikiResponse>>(okResult.Value);
        Assert.True(returnValue.Success);
        Assert.NotNull(returnValue.Data);
    }

    [Fact]
    public async Task DeleteWiki_ReturnsOkResult_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        _wikiServiceMock.Setup(s => s.Delete(wikiId, "user123", false))
            .ReturnsAsync(new Result<WikiDto>(new WikiDto { Id = wikiId }));
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
        var result = await _controller.DeleteWiki(wikiId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<WikiResponse>>(okResult.Value);
        Assert.True(returnValue.Success);
        Assert.NotNull(returnValue.Data);
    }

    [Fact]
    public async Task DeleteWiki_ReturnsBadRequestResult_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        _wikiServiceMock.Setup(s => s.Delete(wikiId, "user123", false)).ReturnsAsync(Result<WikiDto>.NotFound());
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
        var result = await _controller.DeleteWiki(wikiId);

        // Assert
        var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, notFoundResult.StatusCode);
    }
}