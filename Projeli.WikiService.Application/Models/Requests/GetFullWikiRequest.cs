namespace Projeli.WikiService.Application.Models.Requests;

public class GetFullWikiRequest
{
    public bool IncludePages { get; set; } = false;
    public List<string> PageSlugs { get; set; } = [];
    public bool IncludeCategories { get; set; } = false;
    public bool IncludePageCategories { get; set; } = false;
}