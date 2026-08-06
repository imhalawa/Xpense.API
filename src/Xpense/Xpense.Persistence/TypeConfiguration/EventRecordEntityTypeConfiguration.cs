using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xpense.Domain.Entities;

namespace Xpense.Persistence.TypeConfiguration;

public class EventRecordEntityTypeConfiguration : BaseEntityTypeConfiguration<EventRecord>
{
    public override void Configure(EntityTypeBuilder<EventRecord> builder)
    {
        base.Configure(builder);

        // The schema goes in the ToTable call, not a SetSchema before it: the single-argument
        // ToTable overload resets the schema to the default, which silently put this table in
        // `public` while EventPump's claim query names "Xpense"."Events".
        builder.ToTable("Events", XpenseSchema);

        builder.Property(e => e.Type).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Source).HasMaxLength(100).IsRequired();

        // jsonb rather than text: it costs nothing here and means the body can be queried in psql
        // when diagnosing why a rule did or did not fire.
        builder.Property(e => e.Body).HasColumnType("jsonb").IsRequired();

        builder.HasIndex(e => e.EventId).IsUnique();

        // The worker's claim query is "outstanding, oldest first". Filtered so the index holds only
        // the rows being claimed -- processed events accumulate forever and would otherwise dominate it.
        builder.HasIndex(e => e.CreatedAt)
            .HasFilter("\"ProcessedAt\" IS NULL")
            .HasDatabaseName("IX_Events_Outstanding");
    }
}
