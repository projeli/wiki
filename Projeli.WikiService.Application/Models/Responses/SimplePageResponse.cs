using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Models.Responses;

public class SimplePageResponse
{
    public string Title { get; set; }
    
    public string Slug { get; set; }
    
    public PageStatus Status { get; set; }
}