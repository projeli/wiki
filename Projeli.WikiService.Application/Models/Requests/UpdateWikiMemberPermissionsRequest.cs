using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Models.Requests;

public class UpdateWikiMemberPermissionsRequest
{
    public WikiMemberPermissions Permissions { get; set; }
}