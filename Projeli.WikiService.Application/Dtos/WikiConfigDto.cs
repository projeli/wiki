namespace Projeli.WikiService.Application.Dtos;

public class WikiConfigDto
{
    public WikiConfigSidebarDto Sidebar { get; set; }
    
    public class WikiConfigSidebarDto
    {
        public List<WikiConfigSidebarItemDto> Items { get; set; } = [];
        
        public class WikiConfigSidebarItemDto
        {
            public string Index { get; set; }
            public string Title { get; set; }
            public string? Slug { get; set; }
            public List<WikiConfigSidebarItemDto>? Category { get; set; }
        }
    }
}