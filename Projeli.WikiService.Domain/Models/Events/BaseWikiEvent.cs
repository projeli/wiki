using System.Text.Json.Serialization;
using Projeli.WikiService.Domain.Models.Events.Categories;
using Projeli.WikiService.Domain.Models.Events.Members;
using Projeli.WikiService.Domain.Models.Events.Pages;

namespace Projeli.WikiService.Domain.Models.Events;

[JsonDerivedType(typeof(WikiCategoryCreatedEvent), "WikiCategoryCreatedEvent")]
[JsonDerivedType(typeof(WikiCategoryDeletedEvent), "WikiCategoryDeletedEvent")]
[JsonDerivedType(typeof(WikiCategoryUpdatedEvent), "WikiCategoryUpdatedEvent")]
[JsonDerivedType(typeof(WikiMemberAddedEvent), "WikiMemberAddedEvent")]
[JsonDerivedType(typeof(WikiMemberUpdatedPermissionsEvent), "WikiMemberUpdatedPermissionsEvent")]
[JsonDerivedType(typeof(WikiMemberRemovedEvent), "WikiMemberRemovedEvent")]
[JsonDerivedType(typeof(WikiPageCreatedEvent), "WikiPageCreatedEvent")]
[JsonDerivedType(typeof(WikiPageDeletedEvent), "WikiPageDeletedEvent")]
[JsonDerivedType(typeof(WikiPageUpdatedCategoriesEvent), "WikiPageUpdatedCategoriesEvent")]
[JsonDerivedType(typeof(WikiPageUpdatedContentEvent), "WikiPageUpdatedContentEvent")]
[JsonDerivedType(typeof(WikiPageUpdatedDetailsEvent), "WikiPageUpdatedDetailsEvent")]
[JsonDerivedType(typeof(WikiPageUpdatedStatusEvent), "WikiPageUpdatedStatusEvent")]
[JsonDerivedType(typeof(WikiCreatedEvent), "WikiCreatedEvent")]
[JsonDerivedType(typeof(WikiUpdatedContentEvent), "WikiUpdatedContentEvent")]
[JsonDerivedType(typeof(WikiUpdatedOwnershipEvent), "WikiUpdatedOwnershipEvent")]
[JsonDerivedType(typeof(WikiUpdatedSidebarEvent), "WikiUpdatedSidebarEvent")]
[JsonDerivedType(typeof(WikiUpdatedStatusEvent), "WikiUpdatedStatusEvent")]
public abstract class BaseWikiEvent
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTime Timestamp { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ulong Id { get; set; }
    public string UserId { get; set; }
}