namespace Xpense.Adapters.Postgres.Models;

public class Tag
{
    public int TagId { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset? LastModified { get; init; }
    public bool IsDeleted { get; init; }
    public required string Label { get; init; }
    public required string BgColorHex { get; init; }
    public required string FgColorHex { get; init; }

    public Tag With(
        int? tagId = null,
        DateTimeOffset? createdOn = null,
        DateTimeOffset? lastUpdated = null,
        bool? isDeleted = null,
        string? label = null,
        string? bgColorHex = null,
        string? fgColorHex = null
    )
    {
        return new Tag
        {
            TagId = tagId ?? TagId,
            CreatedOn = createdOn ?? CreatedOn,
            LastModified = lastUpdated ?? LastModified,
            IsDeleted = isDeleted ?? IsDeleted,
            Label = label ?? Label,
            BgColorHex = bgColorHex ?? BgColorHex,
            FgColorHex = fgColorHex ?? FgColorHex
        };
    }

    public Tag Duplicate()
    {
        return new Tag
        {
            TagId = TagId,
            CreatedOn = CreatedOn,
            LastModified = LastModified,
            IsDeleted = IsDeleted,
            Label = Label,
            BgColorHex = BgColorHex,
            FgColorHex = FgColorHex
        };
    }
}