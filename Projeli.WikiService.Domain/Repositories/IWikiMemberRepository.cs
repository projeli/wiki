using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiMemberRepository
{
    Task<WikiMember?> Get(Ulid wikiId, string userId);
    Task<WikiMember?> Add(Ulid wikiId, WikiMember member);
    Task<WikiMember?> UpdatePermissions(Ulid wikiId, Ulid userId, WikiMemberPermissions permissions);
    Task<bool> Delete(Ulid wikiId, string userId);
}