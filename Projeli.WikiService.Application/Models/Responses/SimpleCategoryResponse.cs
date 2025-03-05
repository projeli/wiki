namespace Projeli.WikiService.Application.Models.Responses;

public class SimpleCategoryResponse
{
    public Ulid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    public string? Description { get; set; }
}