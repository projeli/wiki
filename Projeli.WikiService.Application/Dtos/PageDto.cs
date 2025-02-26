namespace Projeli.WikiService.Application.Dtos;

public class PageDto
{
    public Ulid Id { get; set; }
    
    public Ulid WikiId { get; set; }
    
    public string Title { get; set; }
    
    public string Slug { get; set; }
    
    public string? Content { get; set; }
    
    public bool IsPublished { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    
    public WikiDto Wiki { get; set; }
    public List<CategoryDto> Categories { get; set; } = [];
    public List<PageVersionDto> Versions { get; set; } = [];
    public List<MemberDto> Editors { get; set; } = [];
}