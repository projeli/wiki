namespace Projeli.WikiService.Application.Dtos;

public class CategoryDto
{
    public Ulid Id { get; set; }
    
    public Ulid WikiId { get; set; }
    
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    
    public WikiDto Wiki { get; set; }
    public List<PageDto> Pages { get; set; } = [];
}