using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Domain.Repositories;

public interface IWikiMemberRepository
{
    Task<WikiMember?> UpdatePermissions(Ulid wikiId, Ulid userId, WikiMemberPermissions permissions);

}