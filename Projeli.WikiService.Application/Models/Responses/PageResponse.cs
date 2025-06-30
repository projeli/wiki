using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Models.Responses;

public class PageResponse
{
    public Ulid Id { get; set; }
    
    public string Title { get; set; }
    
    public string Slug { get; set; }
    
    public PageStatus Status { get; set; }
    
    public string Content { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    public List<SimpleCategoryResponse> Categories { get; set; }
    
    public List<SimpleWikiMemberResponse> Editors { get; set; }
}