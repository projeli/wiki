namespace Projeli.WikiService.Application.Dtos;

public class PageVersionDto
{
    public Ulid Id { get; set; }
    
    public Ulid PageId { get; set; }
    
    public int Version { get; set; }
    
    public string Summary { get; set; }
    
    public string Content { get; set; }
    
    public string Difference { get; set; }
    
    public bool IsPublished { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    
    public PageDto Page { get; set; }
    public List<MemberDto> Editors { get; set; } = [];
}