using Projeli.WikiService.Application.Dtos;

namespace Projeli.WikiService.Application.Models.Requests;

public class UpdateWikiSidebarRequest
{
    public WikiConfigDto.WikiConfigSidebarDto Sidebar { get; set; }
}