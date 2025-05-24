using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Models.Events;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public class WikiEventService(IWikiEventRepository wikiEventRepository, IWikiMemberRepository wikiMemberRepository)
    : IWikiEventService
{
    public async Task<PagedResult<BaseWikiEvent>> GetEvents(Ulid wikiId, List<string> userIds, List<string> eventTypes,
        int page, int pageSize, string? performingUserId, bool force = false)
    {
        if (!force)
        {
            if (string.IsNullOrEmpty(performingUserId))
                return new PagedResult<BaseWikiEvent>([], "User ID is required.", false);
            var performingMember = await wikiMemberRepository.Get(wikiId, performingUserId);
            if (performingMember == null)
                return new PagedResult<BaseWikiEvent>([], "User is not a member of the wiki.", false);
        }

        return await wikiEventRepository.GetEvents(wikiId, userIds, eventTypes, page, pageSize);
    }
}