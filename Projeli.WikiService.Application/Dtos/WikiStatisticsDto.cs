namespace Projeli.WikiService.Application.Dtos;

public class WikiStatisticsDto
{
    public Ulid WikiId { get; set; }
    public int PageCount { get; set; }
    public int CategoryCount { get; set; }
    public int MemberCount { get; set; }
}