using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Models.Responses;

public class SimplePageResponse
{
    public Ulid Id { get; set; }
    
    public string Title { get; set; }
    
    public string Slug { get; set; }
    
    public PageStatus Status { get; set; }
}