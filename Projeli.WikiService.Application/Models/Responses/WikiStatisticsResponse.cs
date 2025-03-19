namespace Projeli.WikiService.Application.Models.Responses;

public class WikiStatisticsResponse
{
    public Ulid WikiId { get; set; }
    public int PageCount { get; set; }
    public int CategoryCount { get; set; }
    public int MemberCount { get; set; }
}