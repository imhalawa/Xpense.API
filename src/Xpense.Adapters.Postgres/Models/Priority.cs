namespace Xpense.Adapters.Postgres.Models
{
    public sealed class Priority
    {
        public int PriorityId { get; init; }
        public DateTimeOffset CreatedOn { get; init; }
        public DateTimeOffset? LastUpdated { get; init; }
        public bool IsDeleted { get; init; }
        public required string Label { get; init; }
        public float Weight { get; init; }

        public Priority With(
            int? priorityId = null,
            DateTimeOffset? createdOn = null,
            DateTimeOffset? lastUpdated = null,
            string? label = null,
            float? weight = null,
            bool? isDeleted = null
        ) => new()
        {
            PriorityId = priorityId ?? PriorityId,
            CreatedOn = createdOn ?? CreatedOn,
            LastUpdated = lastUpdated ?? LastUpdated,
            IsDeleted = isDeleted ?? IsDeleted,
            Label = label ?? Label,
            Weight = weight ?? Weight
        };
    }
}