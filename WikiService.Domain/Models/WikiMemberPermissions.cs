namespace WikiService.Domain.Models;

[Flags]
public enum WikiMemberPermissions : ulong
{
    None = 0,
    EditWikiMemberPermissions = (ulong) 1 << 0,
    CreateWikiPages = (ulong) 1 << 1,
    EditWikiPages = (ulong) 1 << 2,
    PublishWikiPages = (ulong) 1 << 3,
    
    DeleteWikiPages = (ulong) 1 << 10,
    
    DeleteWiki = (ulong) 1 << 63,
    All = ulong.MaxValue
}