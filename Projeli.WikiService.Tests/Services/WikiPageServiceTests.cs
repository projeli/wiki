using AutoMapper;
using Moq;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Profiles;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Tests.Services;

public class WikiPageServiceTests
{
    private readonly Mock<IWikiPageRepository> _repositoryMock;
    private readonly Mock<IWikiRepository> _wikiRepositoryMock;
    private readonly IWikiPageService _service;

    public WikiPageServiceTests()
    {
        _repositoryMock = new Mock<IWikiPageRepository>();
        _wikiRepositoryMock = new Mock<IWikiRepository>();
        Mock<IWikiCategoryRepository> wikiCategoryRepository = new();
        Mock<IBusRepository> busRepository = new();
        Mock<IWikiEventRepository> eventRepository = new();
        var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(WikiProfile))).CreateMapper();
        _service = new Application.Services.WikiPageService(
            _repositoryMock.Object,
            _wikiRepositoryMock.Object,
            wikiCategoryRepository.Object,
            eventRepository.Object,
            busRepository.Object,
            mapper
        );
    }

    [Fact]
    public async Task GetById_ReturnsSuccessResult_WhenWikiPageExists()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        _repositoryMock.Setup(s => s.GetById(wikiPageId, wikiPageId, null, false))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.GetById(wikiPageId, wikiPageId, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPageId, result.Data!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        _repositoryMock.Setup(s => s.GetById(wikiPageId, wikiPageId, null, false))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.GetById(wikiPageId, wikiPageId, null);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetByWikiId_ReturnsSuccessResult_WhenWikiPagesExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var wikiPages = new List<Page>
        {
            new Page
            {
                Id = Ulid.NewUlid(), WikiId = wikiId, Title = "Test Wiki Page 1", Content = "Test Wiki Page Content 1"
            },
            new Page
            {
                Id = Ulid.NewUlid(), WikiId = wikiId, Title = "Test Wiki Page 2", Content = "Test Wiki Page Content 2"
            }
        };
        _repositoryMock.Setup(s => s.GetByWikiId(wikiId, null, false))
            .ReturnsAsync(wikiPages);

        // Act
        var result = await _service.GetByWikiId(wikiId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task GetByWikiId_ReturnsEmptySuccessResult_WhenWikiPagesDoNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        _repositoryMock.Setup(s => s.GetByWikiId(wikiId, null, false))
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetByWikiId(wikiId, null);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data!);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetByProjectId_ReturnsSuccessResult_WhenWikiPagesExist()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        var wikiPages = new List<Page>
        {
            new Page
            {
                Id = Ulid.NewUlid(), WikiId = wikiId, Title = "Test Wiki Page 1", Content = "Test Wiki Page Content 1"
            },
            new Page
            {
                Id = Ulid.NewUlid(), WikiId = wikiId, Title = "Test Wiki Page 2", Content = "Test Wiki Page Content 2"
            }
        };
        _repositoryMock.Setup(s => s.GetByProjectId(projectId, null))
            .ReturnsAsync(wikiPages);

        // Act
        var result = await _service.GetByProjectId(projectId, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task GetByProjectId_ReturnsEmptySuccessResult_WhenWikiPagesDoNotExist()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        _repositoryMock.Setup(s => s.GetByProjectId(projectId, null))
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetByProjectId(projectId, null);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data!);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetByProjectSlug_ReturnsSuccessResult_WhenWikiPagesExist()
    {
        // Arrange
        const string projectSlug = "test-project";
        var wikiId = Ulid.NewUlid();
        var wikiPages = new List<Page>
        {
            new Page
            {
                Id = Ulid.NewUlid(), WikiId = wikiId, Title = "Test Wiki Page 1", Content = "Test Wiki Page Content 1"
            },
            new Page
            {
                Id = Ulid.NewUlid(), WikiId = wikiId, Title = "Test Wiki Page 2", Content = "Test Wiki Page Content 2"
            }
        };
        _repositoryMock.Setup(s => s.GetByProjectSlug(projectSlug, null))
            .ReturnsAsync(wikiPages);

        // Act
        var result = await _service.GetByProjectSlug(projectSlug, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task GetByProjectSlug_ReturnsEmptySuccessResult_WhenWikiPagesDoNotExist()
    {
        // Arrange
        const string projectSlug = "test-project";
        _repositoryMock.Setup(s => s.GetByProjectSlug(projectSlug, null))
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetByProjectSlug(projectSlug, null);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data!);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetBySlug_ReturnsSuccessResult_WhenWikiPageExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        var wikiPage = new Page
        {
            Id = Ulid.NewUlid(),
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Slug = "test-wiki-page",
            Content = "Test Wiki Page Content"
        };
        _repositoryMock.Setup(s => s.GetBySlug(wikiId, "test-wiki-page", null, false))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.GetBySlug(wikiId, "test-wiki-page", null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal("test-wiki-page", result.Data!.Slug);
    }

    [Fact]
    public async Task GetBySlug_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        _repositoryMock.Setup(s => s.GetBySlug(wikiId, "test-wiki-page", null, false))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.GetBySlug(wikiId, "test-wiki-page", null);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetByProjectIdAndId_ReturnsSuccessResult_WhenWikiPageExists()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        var wikiPage = new Page
        {
            Id = Ulid.NewUlid(),
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        _repositoryMock.Setup(s => s.GetByProjectIdAndId(projectId, wikiPage.Id, null))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.GetByProjectIdAndId(projectId, wikiPage.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPage.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByProjectIdAndId_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var wikiPageId = Ulid.NewUlid();
        _repositoryMock.Setup(s => s.GetByProjectIdAndId(projectId, wikiPageId, null))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.GetByProjectIdAndId(projectId, wikiPageId, null);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetByProjectIdAndSlug_ReturnsSuccessResult_WhenWikiPageExists()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        var wikiPage = new Page
        {
            Id = Ulid.NewUlid(),
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Slug = "test-wiki-page",
            Content = "Test Wiki Page Content"
        };
        _repositoryMock.Setup(s => s.GetByProjectIdAndSlug(projectId, wikiPage.Slug, null))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.GetByProjectIdAndSlug(projectId, wikiPage.Slug, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPage.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByProjectIdAndSlug_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var wikiPageSlug = "test-wiki-page";
        _repositoryMock.Setup(s => s.GetByProjectIdAndSlug(projectId, wikiPageSlug, null))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.GetByProjectIdAndSlug(projectId, wikiPageSlug, null);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetByProjectSlugAndId_ReturnsSuccessResult_WhenWikiPageExists()
    {
        // Arrange
        const string projectSlug = "test-project";
        var wikiId = Ulid.NewUlid();
        var wikiPage = new Page
        {
            Id = Ulid.NewUlid(),
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        _repositoryMock.Setup(s => s.GetByProjectSlugAndId(projectSlug, wikiPage.Id, null))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.GetByProjectSlugAndId(projectSlug, wikiPage.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPage.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByProjectSlugAndId_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        const string projectSlug = "test-project";
        var wikiPageId = Ulid.NewUlid();
        _repositoryMock.Setup(s => s.GetByProjectSlugAndId(projectSlug, wikiPageId, null))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.GetByProjectSlugAndId(projectSlug, wikiPageId, null);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetByProjectSlugAndSlug_ReturnsSuccessResult_WhenWikiPageExists()
    {
        // Arrange
        const string projectSlug = "test-project";
        var wikiId = Ulid.NewUlid();
        var wikiPage = new Page
        {
            Id = Ulid.NewUlid(),
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Slug = "test-wiki-page",
            Content = "Test Wiki Page Content"
        };
        _repositoryMock.Setup(s => s.GetByProjectSlugAndSlug(projectSlug, wikiPage.Slug, null))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.GetByProjectSlugAndSlug(projectSlug, wikiPage.Slug, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPage.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByProjectSlugAndSlug_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        const string projectSlug = "test-project";
        const string wikiPageSlug = "test-wiki-page";
        _repositoryMock.Setup(s => s.GetByProjectSlugAndSlug(projectSlug, wikiPageSlug, null))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.GetByProjectSlugAndSlug(projectSlug, wikiPageSlug, null);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }


    [Fact]
    public async Task Create_ReturnsSuccessResult_WhenWikiPageIsCreated()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.CreateWikiPages }]
        };
        var wikiPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Slug = "test-wiki-page",
            Content = "Test Wiki Page Content"
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = wikiPageDto.Title,
            Content = wikiPageDto.Content
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.Create(wikiId, It.IsAny<Page>()))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.Create(wikiId, wikiPageDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPageId, result.Data!.Id);
    }

    [Fact]
    public async Task Create_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wikiPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Slug = "test-wiki-page",
            Content = "Test Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.Create(wikiId, wikiPageDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Create_ThrowsForbiddenException_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wikiPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Slug = "test-wiki-page",
            Content = "Test Wiki Page Content"
        };
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.None }]
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);

        // Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Create(wikiId, wikiPageDto, userId));
    }


    [Fact]
    public async Task Update_ReturnsSuccessResult_WhenWikiPageIsUpdated()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        var newPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = "Updated Wiki Page",
            Slug = "updated-wiki-page",
            Content = "Updated Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);
        _repositoryMock.Setup(s => s.Update(wikiId, It.IsAny<Page>()))
            .ReturnsAsync(() =>
            {
                wikiPage.Title = "Updated Wiki Page";
                return wikiPage;
            });

        // Act
        var result = await _service.Update(wikiId, wikiPageId, newPageDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPageId, result.Data!.Id);
        Assert.Equal("Updated Wiki Page", result.Data!.Title);
    }

    [Fact]
    public async Task Update_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var newPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = "Updated Wiki Page",
            Slug = "updated-wiki-page",
            Content = "Updated Wiki Page Content"
        };
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.Update(wikiId, wikiPageId, newPageDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Update_ThrowsForbiddenException_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.None }]
        };
        var newPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = "Updated Wiki Page",
            Slug = "updated-wiki-page",
            Content = "Updated Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);

        // Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Update(wikiId, wikiPageId, newPageDto, userId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("a")]
    [InlineData("aa")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("Invalid Title#")]
    public async Task Update_ReturnsFailedResult_WhenTitleIsInvalid(string title)
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        var newPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = title,
            Slug = "updated-wiki-page",
            Content = "Updated Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.Update(wikiId, wikiPageId, newPageDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
        Assert.Contains("title", result.Errors.Keys);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("a")]
    [InlineData("aa")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("Invalid Title#")]
    public async Task Update_ReturnsFailedResult_WhenSlugIsInvalid(string slug)
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        var newPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = "Updated Wiki Page",
            Slug = slug,
            Content = "Updated Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);

        // Act
        var result = await _service.Update(wikiId, wikiPageId, newPageDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
        Assert.Contains("slug", result.Errors.Keys);
    }

    [Fact]
    public async Task Update_ReturnsFailedResult_WhenPageSlugIsAlreadyInUse()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        var newPageDto = new PageDto
        {
            WikiId = wikiId,
            Title = "Updated Wiki Page",
            Slug = "updated-wiki-page",
            Content = "Updated Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);
        _repositoryMock.Setup(s => s.GetBySlug(It.IsAny<Ulid>(), It.IsAny<string>(), null, true))
            .ReturnsAsync(new Page { Id = Ulid.NewUlid() });

        // Act
        var result = await _service.Update(wikiId, wikiPageId, newPageDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
        Assert.Contains("slug", result.Errors.Keys);
    }

    [Theory]
    [InlineData(PageStatus.Draft, PageStatus.Published)]
    [InlineData(PageStatus.Published, PageStatus.Archived)]
    [InlineData(PageStatus.Archived, PageStatus.Published)]
    public async Task UpdateStatus_ReturnsSuccessResult_WhenWikiPageStatusIsUpdated(PageStatus currentStatus,
        PageStatus newStatus)
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.PublishWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content",
            Status = currentStatus
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);
        _repositoryMock.Setup(s => s.UpdateStatus(wikiId, wikiPageId, newStatus))
            .ReturnsAsync(() =>
            {
                wikiPage.Status = newStatus;
                return wikiPage;
            });

        // Act
        var result = await _service.UpdateStatus(wikiId, wikiPageId, newStatus, userId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPageId, result.Data!.Id);
        Assert.Equal(newStatus, result.Data!.Status);
    }

    [Theory]
    [InlineData(PageStatus.Draft, PageStatus.Archived)]
    [InlineData(PageStatus.Draft, PageStatus.Draft)]
    [InlineData(PageStatus.Published, PageStatus.Draft)]
    [InlineData(PageStatus.Published, PageStatus.Published)]
    [InlineData(PageStatus.Archived, PageStatus.Draft)]
    [InlineData(PageStatus.Archived, PageStatus.Archived)]
    public async Task UpdateStatus_ReturnsFailedResult_WhenWikiPageStatusIsNotAllowed(PageStatus currentStatus,
        PageStatus newStatus)
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.PublishWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content",
            Status = currentStatus
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);

        // Assert
        var result = await _service.UpdateStatus(wikiId, wikiPageId, newStatus, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task UpdateStatus_ThrowsForbiddenException_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.None }]
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);

        // Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.UpdateStatus(wikiId, wikiPageId, PageStatus.Published, userId));
    }


    [Fact]
    public async Task UpdateContent_ReturnsSuccessResult_WhenWikiPageIsUpdated()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);
        _repositoryMock.Setup(s => s.UpdateContent(wikiId, It.IsAny<Page>()))
            .ReturnsAsync(() =>
            {
                wikiPage.Content = "Updated Wiki Page Content";
                return wikiPage;
            });

        // Act
        var result = await _service.UpdateContent(wikiId, wikiPageId, "Updated Wiki Page Content", userId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPageId, result.Data!.Id);
        Assert.Equal("Updated Wiki Page Content", result.Data!.Content);
    }

    [Fact]
    public async Task UpdateContent_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.UpdateContent(wikiId, wikiPageId, "Updated Wiki Page Content", userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task UpdateContent_ReturnsFailedResult_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.None }]
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);

        // Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.UpdateContent(wikiId, wikiPageId, "Updated Wiki Page Content", userId));
    }

    [Fact]
    public async Task UpdateContent_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wikiPageId = Ulid.NewUlid();
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateContent(wikiId, wikiPageId, "Updated Wiki Page Content", userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task UpdateCategories_ReturnsSuccessResult_WhenWikiPageCategoriesAreUpdated()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);
        _repositoryMock.Setup(s => s.UpdateCategories(wikiId, It.IsAny<Page>(), It.IsAny<List<Ulid>>()))
            .ReturnsAsync(() =>
            {
                wikiPage.Categories = [new Category { Id = Ulid.NewUlid(), Name = "Test Category" }];
                return wikiPage;
            });

        // Act
        var result = await _service.UpdateCategories(wikiId, wikiPageId, [Ulid.NewUlid()], userId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPageId, result.Data!.Id);
        Assert.Single(result.Data!.Categories);
    }

    [Fact]
    public async Task UpdateCategories_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.UpdateCategories(wikiId, wikiPageId, [Ulid.NewUlid()], userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task UpdateCategories_ReturnsFailedResult_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.None }]
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);

        // Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.UpdateCategories(wikiId, wikiPageId, [Ulid.NewUlid()], userId));
    }

    [Fact]
    public async Task UpdateCategories_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wikiPageId = Ulid.NewUlid();
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateCategories(wikiId, wikiPageId, [Ulid.NewUlid()], userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Delete_ReturnsSuccessResult_WhenWikiPageIsDeleted()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.DeleteWikiPages }]
        };
        var wikiPage = new Page
        {
            Id = wikiPageId,
            WikiId = wikiId,
            Title = "Test Wiki Page",
            Content = "Test Wiki Page Content"
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync(wikiPage);
        _repositoryMock.Setup(s => s.Delete(wikiId, wikiPageId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.Delete(wikiId, wikiPageId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal(wikiPageId, result.Data!.Id);
    }

    [Fact]
    public async Task Delete_ReturnsFailedResult_WhenWikiPageDoesNotExist()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        _repositoryMock.Setup(s => s.GetById(wikiId, wikiPageId, userId, false))
            .ReturnsAsync((Page?)null);

        // Act
        var result = await _service.Delete(wikiId, wikiPageId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Delete_ReturnsFailedResult_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var wikiPageId = Ulid.NewUlid();
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-project",
            Status = WikiStatus.Published,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.None }]
        };
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);

        // Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Delete(wikiId, wikiPageId, userId));
    }

    [Fact]
    public async Task Delete_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wikiPageId = Ulid.NewUlid();
        _wikiRepositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.Delete(wikiId, wikiPageId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.False(result.Success);
    }
}