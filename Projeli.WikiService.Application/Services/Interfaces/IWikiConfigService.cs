﻿using Projeli.Shared.Domain.Results;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Services.Interfaces;

public interface IWikiConfigService
{
    Task<IResult<WikiConfigDto?>> GetByWikiId(Ulid wikiId, string? userId);
}