using AutoMapper;
using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Services.Interfaces;
using Projeli.WikiService.Domain.Repositories;

namespace Projeli.WikiService.Application.Services;

public class WikiConfigService(IWikiConfigRepository repository, IMapper mapper) : IWikiConfigService
{
    public async Task<IResult<WikiConfigDto?>> GetByWikiId(Ulid wikiId, string? userId)
    {
        var wikiConfig = await repository.GetByWikiId(wikiId, userId);
        return wikiConfig is not null
            ? new Result<WikiConfigDto?>(mapper.Map<WikiConfigDto>(wikiConfig))
            : Result<WikiConfigDto?>.NotFound();
    }
}