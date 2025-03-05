namespace Projeli.WikiService.Domain.Models;

public enum WikiStatus : ushort
{
    Uncreated = 0,
    Draft = 1,
    Published = 2,
    Archived = 3
}