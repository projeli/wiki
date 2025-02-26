namespace Projeli.WikiService.Application.Dtos;

public class WikiDto
{
    public Ulid Id { get; set; }
    
    public Ulid ProjectId { get; set; }
    
    public string ProjectName { get; set; }
    
    public string ProjectSlug { get; set; }
    
    public bool IsPublished { get; set; }
 
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    public List<MemberDto> Members { get; set; } = [];
    public List<CategoryDto> Categories { get; set; } = [];
    public List<PageDto> Pages { get; set; } = [];
}