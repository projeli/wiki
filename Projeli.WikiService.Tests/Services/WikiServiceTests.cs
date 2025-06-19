using AutoMapper;
using Moq;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Profiles;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Tests.Services;

public class WikiServiceTests
{
    private readonly Mock<IWikiRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly IWikiService _service;

    public WikiServiceTests()
    {
        _repositoryMock = new Mock<IWikiRepository>();
        Mock<IWikiEventRepository> eventRepositoryMock = new();
        Mock<IBusRepository> busRepository = new();
        _mapper =
            new MapperConfiguration(cfg => cfg.AddMaps(typeof(WikiProfile))).CreateMapper();
        _service = new Application.Services.WikiService(_repositoryMock.Object, busRepository.Object, _mapper,
            eventRepositoryMock.Object);
    }

    [Fact]
    public async Task GetById_ReturnsSuccessResult_WhenWikiExists()
    {
        // Arrange
        var wiki = new Wiki();
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.GetById(Ulid.NewUlid(), null);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetById_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.GetById(Ulid.NewUlid(), null);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByProjectId_ReturnsSuccessResult_WhenWikiExists()
    {
        // Arrange
        var wiki = new Wiki();
        _repositoryMock.Setup(s => s.GetByProjectId(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.GetByProjectId(Ulid.NewUlid(), null);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByProjectId_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(s => s.GetByProjectId(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.GetByProjectId(Ulid.NewUlid(), null);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByProjectSlug_ReturnsSuccessResult_WhenWikiExists()
    {
        // Arrange
        var wiki = new Wiki();
        _repositoryMock.Setup(s => s.GetByProjectSlug(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.GetByProjectSlug("test-slug", null);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByProjectSlug_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(s => s.GetByProjectSlug(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.GetByProjectSlug("test-slug", null);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetStatistics_ReturnsSuccessResult_WhenWikiExists()
    {
        // Arrange
        var wikiStatistics = new WikiStatistics();
        _repositoryMock.Setup(s => s.GetStatistics(It.IsAny<Ulid>(), It.IsAny<string>()))
            .ReturnsAsync(wikiStatistics);

        // Act
        var result = await _service.GetStatistics(Ulid.NewUlid(), null);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetStatistics_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(s => s.GetStatistics(It.IsAny<Ulid>(), It.IsAny<string>()))
            .ReturnsAsync((WikiStatistics?)null);

        // Act
        var result = await _service.GetStatistics(Ulid.NewUlid(), null);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Create_ReturnsSuccessResult_WhenWikiIsValid()
    {
        // Arrange
        var wikiDto = new WikiDto
        {
            ProjectId = Ulid.NewUlid(),
            ProjectSlug = "test-slug",
            ProjectName = "Test Project",
            Members = [new WikiMemberDto { UserId = "user123", IsOwner = true }]
        };
        var wiki = _mapper.Map<Wiki>(wikiDto);
        _repositoryMock.Setup(s => s.Create(It.IsAny<Wiki>())).ReturnsAsync(wiki);

        // Act
        var result = await _service.Create(wikiDto.ProjectId, wikiDto.ProjectSlug, wikiDto.ProjectName,
            wikiDto.ProjectImageUrl, wikiDto.Members,
            "user123");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Create_ReturnsFailedResult_WhenWikiIsInvalid()
    {
        // Arrange
        var wikiDto = new WikiDto();
        var wiki = _mapper.Map<Wiki>(wikiDto);
        _repositoryMock.Setup(s => s.Create(It.IsAny<Wiki>()))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.Create(Ulid.Empty, "", "", "",
            [new WikiMemberDto { UserId = "user123", IsOwner = true }], "user123");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateProjectInfo_ReturnsSuccessResult_WhenWikiExists()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var existingWiki = new Wiki()
        {
            ProjectId = projectId,
            ProjectSlug = "test-slug",
            ProjectName = "Test Project",
            ProjectImageUrl = "https://example.com/image.png",
        };
        var wikiDto = new WikiDto
        {
            ProjectId = projectId,
            ProjectSlug = "new-test-slug",
            ProjectName = "New Test Project",
            ProjectImageUrl = "https://example.com/new-image.png"
        };
        var updatedWiki = new Wiki()
        {
            ProjectId = projectId,
            ProjectSlug = "new-test-slug",
            ProjectName = "New Test Project",
            ProjectImageUrl = "https://example.com/new-image.png",
        };
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(existingWiki);
        _repositoryMock.Setup(s => s.Update(It.IsAny<Wiki>()))
            .ReturnsAsync(updatedWiki);

        // Act
        var result = await _service.UpdateProjectDetails(Ulid.NewUlid(), wikiDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateProjectInfo_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateProjectDetails(Ulid.NewUlid(), new WikiDto());

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateProjectInfo_ReturnsFailedResult_WhenUpdateFails()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var existingWiki = new Wiki()
        {
            ProjectId = projectId,
            ProjectSlug = "test-slug",
            ProjectName = "Test Project",
            ProjectImageUrl = "https://example.com/image.png",
        };
        var wikiDto = new WikiDto
        {
            ProjectId = projectId,
            ProjectSlug = "new-test-slug",
            ProjectName = "New Test Project",
            ProjectImageUrl = "https://example.com/image.png"
        };
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(existingWiki);
        _repositoryMock.Setup(s => s.Update(It.IsAny<Wiki>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateProjectDetails(Ulid.NewUlid(), wikiDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateProjectInfo_ReturnsFailedResult_WhenWikiIsInvalid()
    {
        // Arrange
        Wiki? wiki = null;
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.UpdateProjectDetails(Ulid.NewUlid(), new WikiDto { Id = new Ulid() });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Theory]
    [InlineData(WikiStatus.Draft, WikiStatus.Published)]
    [InlineData(WikiStatus.Published, WikiStatus.Archived)]
    [InlineData(WikiStatus.Archived, WikiStatus.Published)]
    public async Task UpdateStatus_ReturnsSuccessResult_WhenWikiExists(WikiStatus currentStatus, WikiStatus newStatus)
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            Status = currentStatus,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWiki }]
        };
        _repositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.UpdateStatus(wikiId, newStatus))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.UpdateStatus(wikiId, newStatus, userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Theory]
    [InlineData(WikiStatus.Draft, WikiStatus.Draft)]
    [InlineData(WikiStatus.Draft, WikiStatus.Archived)]
    [InlineData(WikiStatus.Published, WikiStatus.Published)]
    [InlineData(WikiStatus.Published, WikiStatus.Draft)]
    [InlineData(WikiStatus.Archived, WikiStatus.Archived)]
    [InlineData(WikiStatus.Archived, WikiStatus.Draft)]
    public async Task UpdateStatus_ReturnsFailedResult_WhenStatusNotAllowed(WikiStatus currentStatus,
        WikiStatus newStatus)
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            Status = currentStatus,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWiki }]
        };
        _repositoryMock.Setup(s => s.GetById(wikiId, userId, false))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.UpdateStatus(wikiId, newStatus))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.UpdateStatus(wikiId, newStatus, userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateStatus(wikiId, WikiStatus.Published, userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsFailedResult_WhenUpdateFails()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWiki }]
        };
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.UpdateStatus(It.IsAny<Ulid>(), It.IsAny<WikiStatus>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateStatus(wikiId, WikiStatus.Published, userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateContent_ReturnsSuccessResult_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWiki }]
        };
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.UpdateContent(It.IsAny<Ulid>(), It.IsAny<string>()))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.UpdateContent(wikiId, "Test content", userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateContent_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateContent(wikiId, "Test content", userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateContent_ReturnsFailedResult_WhenUpdateFails()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWiki }]
        };
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.UpdateContent(It.IsAny<Ulid>(), It.IsAny<string>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateContent(wikiId, "Test content", userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateSidebar_ReturnsSuccessResult_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWiki }],
        };
        var newWikiSidebar = new WikiConfigDto.WikiConfigSidebarDto
        {
            Items =
            [
                new WikiConfigDto.WikiConfigSidebarDto.WikiConfigSidebarItemDto
                {
                    Index = "2",
                    Title = "Test2",
                    Category =
                    [
                        new WikiConfigDto.WikiConfigSidebarDto.WikiConfigSidebarItemDto
                        {
                            Index = "1",
                            Title = "Test2",
                            Slug = "test2"
                        },
                        new WikiConfigDto.WikiConfigSidebarDto.WikiConfigSidebarItemDto
                        {
                            Index = "1",
                            Title = "Test3"
                        }
                    ]
                },
                new WikiConfigDto.WikiConfigSidebarDto.WikiConfigSidebarItemDto
                {
                    Index = "3",
                    Title = "Test",
                    Slug = "test"
                },
                new WikiConfigDto.WikiConfigSidebarDto.WikiConfigSidebarItemDto
                {
                    Index = "4",
                    Title = "Test3"
                }
            ]
        };
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.UpdateSidebar(It.IsAny<Ulid>(), It.IsAny<WikiConfig.WikiConfigSidebar>()))
            .ReturnsAsync(wiki);

        // Act
        var result = await _service.UpdateSidebar(wikiId, newWikiSidebar, userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateSidebar_ReturnsFailedResult_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateSidebar(wikiId, new WikiConfigDto.WikiConfigSidebarDto(), userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateSidebar_ReturnsFailedResult_WhenUpdateFails()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.EditWiki }]
        };
        _repositoryMock.Setup(s => s.GetById(It.IsAny<Ulid>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.UpdateSidebar(It.IsAny<Ulid>(), It.IsAny<WikiConfig.WikiConfigSidebar>()))
            .ReturnsAsync((Wiki?)null);

        // Act
        var result = await _service.UpdateSidebar(wikiId, new WikiConfigDto.WikiConfigSidebarDto(), userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Delete_ReturnsTrue_WhenWikiExists()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        var wiki = new Wiki
        {
            Id = wikiId,
            Members = [new WikiMember { UserId = userId, Permissions = WikiMemberPermissions.DeleteWiki }]
        };
        _repositoryMock.Setup(s => s.GetById(wikiId, userId, false)).ReturnsAsync(wiki);
        _repositoryMock.Setup(s => s.Delete(wikiId)).ReturnsAsync(true);

        // Act
        var result = await _service.Delete(wikiId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenWikiDoesNotExist()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        _repositoryMock.Setup(s => s.Delete(It.IsAny<Ulid>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.Delete(wikiId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenDeleteFails()
    {
        // Arrange
        var wikiId = Ulid.NewUlid();
        const string userId = "user123";
        _repositoryMock.Setup(s => s.Delete(It.IsAny<Ulid>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.Delete(wikiId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);
    }
}