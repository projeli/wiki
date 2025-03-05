using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Models.Requests;

public class UpdateWikiStatusRequest
{
    public WikiStatus Status { get; set; }
}