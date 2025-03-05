namespace Projeli.WikiService.Application.Dtos;

public class WikiConfigDto
{
    public WikiConfigSidebarDto Sidebar { get; set; }
    
    public class WikiConfigSidebarDto
    {
        public List<WikiConfigSidebarItemDto> Items { get; set; } = [];
        
        public class WikiConfigSidebarItemDto
        {
            public string Title { get; set; }
            public string? Href { get; set; }
            public List<WikiConfigSidebarItemDto>? Category { get; set; } 
        }
    }
}