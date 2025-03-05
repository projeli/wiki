using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Models.Responses;

public class WikiResponse
{
    public Ulid Id { get; set; }
    
    public Ulid ProjectId { get; set; }
    
    public string ProjectName { get; set; }
    
    public string ProjectSlug { get; set; }
    
    public string? Name { get; set; }
    
    public string? Content { get; set; }
    
    public WikiConfigDto Config { get; set; }
    
    public WikiStatus Status { get; set; }
 
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    public List<SimpleWikiMemberResponse> Members { get; set; } = [];
    public List<SimpleCategoryResponse> Categories { get; set; } = [];
}