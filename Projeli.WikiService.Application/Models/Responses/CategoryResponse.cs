namespace Projeli.WikiService.Application.Models.Responses;

public class CategoryResponse
{
    public Ulid Id { get; set; }
    
    public Ulid WikiId { get; set; }
    
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public List<SimplePageResponse> Pages { get; set; } = [];
}