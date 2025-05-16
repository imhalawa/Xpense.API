namespace Xpense.Adapters.Postgres.Models;

public sealed class Category
{
    public int CategoryId { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset? LastUpdated { get; init; }
    public bool IsDeleted { get; init; }
    public required string Label { get; init; }
    public int PriorityId { get; init; }
    public Priority? Priority { get; init; }

    public Category With(
        int? categoryId = null,
        DateTimeOffset? createdOn = null,
        DateTimeOffset? lastUpdated = null,
        bool? isDeleted = null,
        string? label = null,
        int? priorityId = null,
        Priority? priority = null
    ) => new()
    {
        CategoryId = categoryId ?? CategoryId,
        CreatedOn = createdOn ?? CreatedOn,
        LastUpdated = lastUpdated ?? LastUpdated,
        IsDeleted = isDeleted ?? IsDeleted,
        Label = label ?? Label,
        PriorityId = priorityId ?? PriorityId,
        Priority = priority ?? Priority
    };
}