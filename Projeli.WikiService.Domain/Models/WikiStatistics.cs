namespace Projeli.WikiService.Domain.Models;

public class WikiStatistics
{
    public Ulid WikiId { get; set; }
    public int PageCount { get; set; }
    public int CategoryCount { get; set; }
    public int MemberCount { get; set; }
}