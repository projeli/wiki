using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Models.Requests;

public class UpdatePageStatusRequest
{
    public PageStatus Status { get; set; }
}