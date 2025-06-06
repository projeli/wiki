﻿using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiRepository
{
    Task<List<Wiki>> GetByIds(List<Ulid> ids, string? userId);
    Task<Wiki?> GetById(Ulid id, string? userId, bool force = false);
    Task<Wiki?> GetByProjectId(Ulid projectId, string? userId, bool force = false);
    Task<Wiki?> GetByProjectSlug(string projectSlug, string? userId, bool force = false);
    Task<WikiStatistics?> GetStatistics(Ulid id, string? userId);
    Task<Wiki?> Create(Wiki wiki);
    Task<Wiki?> Update(Wiki wiki);
    Task<Wiki?> UpdateStatus(Ulid id, WikiStatus status);
    Task<Wiki?> UpdateContent(Ulid id, string content);
    Task<Wiki?> UpdateSidebar(Ulid id, WikiConfig.WikiConfigSidebar sidebar);
    Task<Wiki?> UpdateOwnership(Ulid id, string oldOwnerUserId, string newOwnerUserId, WikiMemberPermissions oldOwnerPermissions, WikiMemberPermissions newOwnerPermissions);
    Task<bool> Delete(Ulid id);
}