using AutoMapper;
using Projeli.Shared.Application.Exceptions.Http;
using Projeli.Shared.Application.Messages.Notifications;
using Projeli.Shared.Domain.Models.Notifications;
using Projeli.Shared.Domain.Models.Notifications.Types.Wikis;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models;
using Projeli.WikiService.Domain.Models.Events;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public class WikiService(IWikiRepository repository, IBusRepository busRepository, IMapper mapper, IWikiEventRepository wikiEventRepository) : IWikiService
{
    public async Task<IResult<List<WikiDto>>> GetByIds(List<Ulid> ids, string? userId)
    {
        var wikis = await repository.GetByIds(ids, userId);
        return new Result<List<WikiDto>>(mapper.Map<List<WikiDto>>(wikis));
    }

    public async Task<IResult<WikiDto?>> GetById(Ulid id, string? userId, bool force = false)
    {
        var wiki = await repository.GetById(id, userId, force);
        return wiki is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(wiki))
            : Result<WikiDto?>.NotFound();
    }

    public async Task<IResult<WikiDto?>> GetByProjectId(Ulid projectId, string? userId, bool force = false)
    {
        var result = await repository.GetByProjectId(projectId, userId, force);
        return result is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(result))
            : Result<WikiDto?>.NotFound();
    }

    public async Task<IResult<WikiDto?>> GetByProjectSlug(string projectSlug, string? userId, bool force = false)
    {
        var result = await repository.GetByProjectSlug(projectSlug, userId, force);
        return result is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(result))
            : Result<WikiDto?>.NotFound();
    }

    public async Task<IResult<WikiStatisticsDto?>> GetStatistics(Ulid id, string? userId)
    {
        var result = await repository.GetStatistics(id, userId);
        return result is not null
            ? new Result<WikiStatisticsDto?>(mapper.Map<WikiStatisticsDto>(result))
            : Result<WikiStatisticsDto?>.NotFound();
    }

    public async Task<IResult<WikiDto?>> Create(Ulid projectId, string projectName, string projectSlug, 
        string? projectImageUrl, List<WikiMemberDto> members, string userId)
    {
        var performingMember = members.FirstOrDefault(member => member.UserId == userId);
        if (performingMember is null || !performingMember.IsOwner)
        {
            throw new ForbiddenException("You do not have permission to create this wiki");
        }

        var wikiId = Ulid.NewUlid();
        var wikiDto = new WikiDto
        {
            Id = wikiId,
            ProjectId = projectId,
            ProjectName = projectName,
            ProjectSlug = projectSlug,
            ProjectImageUrl = projectImageUrl,
            Members = members.Select(member => new WikiMemberDto
            {
                Id = Ulid.NewUlid(),
                WikiId = wikiId,
                UserId = member.UserId,
                IsOwner = member.IsOwner,
                Permissions = member.IsOwner
                    ? WikiMemberPermissions.All
                    : WikiMemberPermissions.None,
            }).ToList(),
        };

        var validationResult = ValidateWiki(wikiDto);
        if (validationResult.Failed) return validationResult;

        var wiki = mapper.Map<Wiki>(wikiDto);
        var createdWiki = await repository.Create(wiki);

        if (createdWiki is not null)
        {
            await wikiEventRepository.StoreEvent(createdWiki.Id, new WikiCreatedEvent
            {
                UserId = userId,
                Status = createdWiki.Status,
            });
        }

        return createdWiki is not null
            ? new Result<WikiDto?>(mapper.Map<WikiDto>(createdWiki))
            : Result<WikiDto?>.Fail("Failed to create wiki");
    }

    public async Task<IResult<WikiDto?>> UpdateProjectDetails(Ulid id, WikiDto wikiDto)
    {
        var existingWiki = await repository.GetById(id, null, true);
        if (existingWiki is null) return Result<WikiDto?>.NotFound();

        if (existingWiki.ProjectName.Equals(wikiDto.ProjectName) &&
            existingWiki.ProjectSlug.Equals(wikiDto.ProjectSlug))
        {
            return new Result<WikiDto>(mapper.Map<WikiDto>(existingWiki), "Project details are the same");
        }
        
        existingWiki.ProjectName = wikiDto.ProjectName;
        existingWiki.ProjectSlug = wikiDto.ProjectSlug;

        var updatedWiki = await repository.Update(existingWiki);

        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }

    public async Task<IResult<WikiDto?>> UpdateStatus(Ulid id, WikiStatus status, string userId)
    {
        var existingWiki = await repository.GetById(id, userId);
        if (existingWiki is null) return Result<WikiDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
        if (!CanEditWikiStatus(member, status))
        {
            throw new ForbiddenException("You do not have permission to edit this wiki");
        }

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (status)
        {
            case WikiStatus.Draft:
                return Result<WikiDto>.Fail("Cannot set status to draft");
            case WikiStatus.Published
                when existingWiki.Status != WikiStatus.Draft && existingWiki.Status != WikiStatus.Archived:
                return Result<WikiDto>.Fail("Cannot set status to published");
            case WikiStatus.Archived when existingWiki.Status != WikiStatus.Published:
                return Result<WikiDto>.Fail("Cannot set status to archived");
        }

        var updatedWiki = await repository.UpdateStatus(id, status);

        if (updatedWiki is not null)
        {
            await wikiEventRepository.StoreEvent(id, new WikiUpdatedStatusEvent
            {
                UserId = userId,
                Status = updatedWiki.Status,
            });

            if (updatedWiki.Status == WikiStatus.Published)
            {
                await busRepository.Publish(new AddNotificationsMessage
                {
                    Notifications = existingWiki.Members.Select(x => new NotificationMessage
                    {
                        UserId = x.UserId,
                        Type = NotificationType.WikiPublished,
                        Body = new WikiPublished
                        {
                            PerformerId = userId,
                            WikiId = updatedWiki.Id
                        },
                        IsRead = x.UserId == userId,
                    }).ToList(),
                });
            } else if (updatedWiki.Status == WikiStatus.Archived)
            {
                await busRepository.Publish(new AddNotificationsMessage
                {
                    Notifications = existingWiki.Members.Select(x => new NotificationMessage
                    {
                        UserId = x.UserId,
                        Type = NotificationType.WikiArchived,
                        Body = new WikiArchived
                        {
                            PerformerId = userId,
                            WikiId = updatedWiki.Id
                        },
                        IsRead = x.UserId == userId,
                    }).ToList(),
                });
            }
        }

        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }
    
    private static bool CanEditWikiStatus(WikiMember? member, WikiStatus status)
    {
        if (member == null) return false;
        if (member.IsOwner) return true;

        return status switch
        {
            WikiStatus.Published => member.Permissions.HasFlag(WikiMemberPermissions.PublishWiki),
            WikiStatus.Archived => member.Permissions.HasFlag(WikiMemberPermissions.ArchiveWiki),
            _ => false
        };
    }

    public async Task<IResult<WikiDto?>> UpdateContent(Ulid id, string content, string userId)
    {
        var existingWiki = await repository.GetById(id, userId);
        if (existingWiki is null) return Result<WikiDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
        if (member is null || (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWiki)))
        {
            throw new ForbiddenException("You do not have permission to edit this wiki");
        }

        if (existingWiki.Content?.Equals(content) == true)
        {
            return new Result<WikiDto>(mapper.Map<WikiDto>(existingWiki), "Content is the same");
        }

        var updatedWiki = await repository.UpdateContent(id, content);

        if (updatedWiki is not null)
        {
            await wikiEventRepository.StoreEvent(id, new WikiUpdatedContentEvent
            {
                UserId = userId,
                Content = updatedWiki.Content,
            });
        }

        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }

    public async Task<IResult<WikiDto?>> UpdateSidebar(Ulid id, WikiConfigDto.WikiConfigSidebarDto sidebarDto,
        string userId)
    {
        var existingWiki = await repository.GetById(id, userId);
        if (existingWiki is null) return Result<WikiDto>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
        if (member is null || (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.EditWiki)))
        {
            throw new ForbiddenException("You do not have permission to edit this wiki");
        }

        for (var i = sidebarDto.Items.Count - 1; i >= 0; i--)
        {
            var item = sidebarDto.Items[i];

            if (item.Slug is not null)
            {
                item.Category = null;
            }
            else if (item.Category is not null)
            {
                item.Slug = null;
                item.Index = Ulid.NewUlid().ToString();

                if (item.Category != null)
                {
                    for (var j = item.Category.Count - 1; j >= 0; j--)
                    {
                        var subItem = item.Category[j];
                        if (subItem.Slug is not null)
                        {
                            subItem.Category = null;
                        }
                        else
                        {
                            item.Category.RemoveAt(j);
                        }
                    }
                }
            }
            else
            {
                sidebarDto.Items.RemoveAt(i);
            }
        }

        var sidebar = mapper.Map<WikiConfig.WikiConfigSidebar>(sidebarDto);

        if (existingWiki.Config?.Sidebar?.Equals(sidebar) == true)
        {
            return new Result<WikiDto>(mapper.Map<WikiDto>(existingWiki), "Sidebar is the same");
        }

        var updatedWiki = await repository.UpdateSidebar(id, sidebar);

        if (updatedWiki is not null)
        {
            await wikiEventRepository.StoreEvent(id, new WikiUpdatedSidebarEvent
            {
                UserId = userId,
                Sidebar = updatedWiki.Config?.Sidebar ?? new WikiConfig.WikiConfigSidebar(),
            });
        }

        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }

    public async Task<IResult<WikiDto?>> UpdateOwnership(Ulid wikiId, string fromUserId, string toUserId,
        bool force = false)
    {
        var existingWiki = await repository.GetById(wikiId, fromUserId, force);
        if (existingWiki is null) return Result<WikiDto?>.NotFound();

        var member = existingWiki.Members.FirstOrDefault(member => member.UserId == fromUserId);
        if (member is null)
        {
            return Result<WikiDto?>.NotFound();
        }

        if (!force && !member.IsOwner)
        {
            throw new ForbiddenException("You do not have permission to edit this wiki");
        }

        var oldOwnerPermissions = Enum.GetValues(typeof(WikiMemberPermissions))
            .Cast<WikiMemberPermissions>()
            .Where(p => p != WikiMemberPermissions.All)
            .Aggregate(WikiMemberPermissions.None, (current, permission) => current | permission);

        var updatedWiki = await repository.UpdateOwnership(wikiId, fromUserId, toUserId, oldOwnerPermissions,
            WikiMemberPermissions.All);

        if (updatedWiki is not null)
        {
            await wikiEventRepository.StoreEvent(wikiId, new WikiUpdatedOwnershipEvent
            {
                UserId = fromUserId,
                ToUserId = toUserId,
            });
        }

        return updatedWiki is not null
            ? new Result<WikiDto>(mapper.Map<WikiDto>(updatedWiki))
            : Result<WikiDto>.Fail("Failed to update project");
    }

    public async Task<IResult<WikiDto?>> Delete(Ulid id, string? userId, bool force = false)
    {
        var existingWiki = await repository.GetById(id, userId, force);
        if (existingWiki is null) return Result<WikiDto>.NotFound();

        if (!force)
        {
            var member = existingWiki.Members.FirstOrDefault(member => member.UserId == userId);
            if (member is null || (!member.IsOwner && !member.Permissions.HasFlag(WikiMemberPermissions.DeleteWiki)))
            {
                throw new ForbiddenException("You do not have permission to delete this wiki");
            }
        }

        var success = await repository.Delete(id);

        if (success)
        {
            await wikiEventRepository.DeleteEvents(id);

            // If userId is null, the system is deleting the wiki meaning that the project has been deleted
            // That actions will create a notification that the project has been deleted so we don't need to create
            // a notification for the wiki deletion.
            if (userId is not null)
            {
                await busRepository.Publish(new AddNotificationsMessage
                {
                    Notifications = existingWiki.Members.Select(x => new NotificationMessage
                    {
                        UserId = x.UserId,
                        Type = NotificationType.WikiDeleted,
                        Body = new WikiDeleted
                        {
                            PerformerId = userId,
                            WikiId = existingWiki.Id,
                            WikiName = existingWiki.ProjectName,
                        },
                        IsRead = x.UserId == userId,
                    }).ToList(),
                });
            }
        }

        return success
            ? new Result<WikiDto>(mapper.Map<WikiDto>(existingWiki))
            : Result<WikiDto>.Fail("Failed to delete project");
    }

    private static IResult<WikiDto?> ValidateWiki(WikiDto wikiDto)
    {
        var errors = new Dictionary<string, string[]>();

        if (wikiDto.Id == Ulid.Empty)
        {
            errors.Add(nameof(wikiDto.Id), ["Id is required"]);
        }

        if (wikiDto.ProjectId == Ulid.Empty)
        {
            errors.Add(nameof(wikiDto.ProjectId), ["ProjectId is required"]);
        }

        if (string.IsNullOrWhiteSpace(wikiDto.ProjectSlug))
        {
            errors.Add(nameof(wikiDto.ProjectSlug), ["ProjectSlug is required"]);
        }

        if (wikiDto.Members.Count == 0)
        {
            errors.Add(nameof(wikiDto.Members), ["Members are required"]);
        }

        return errors.Count > 0
            ? Result<WikiDto?>.ValidationFail(errors)
            : new Result<WikiDto?>(null);
    }
}