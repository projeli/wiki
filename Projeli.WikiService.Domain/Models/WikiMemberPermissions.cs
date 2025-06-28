namespace Projeli.WikiService.Domain.Models;

[Flags]
public enum WikiMemberPermissions : ulong
{
    None = 0,
    EditWikiMemberPermissions = (ulong) 1 << 0,
    
    EditWiki = (ulong) 1 << 1,
    PublishWiki = (ulong) 1 << 2,
    ArchiveWiki = (ulong) 1 << 3,
    
    CreateWikiPages = (ulong) 1 << 11,
    EditWikiPages = (ulong) 1 << 12,
    PublishWikiPages = (ulong) 1 << 13,
    ArchiveWikiPages = (ulong) 1 << 14,
    
    DeleteWikiPages = (ulong) 1 << 20,
    
    CreateWikiCategories = (ulong) 1 << 21,
    EditWikiCategories = (ulong) 1 << 22,
    DeleteWikiCategories = (ulong) 1 << 30,
    
    DeleteWiki = (ulong) 1 << 63,
    All = ulong.MaxValue
}