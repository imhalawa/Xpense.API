using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xpense.Domain.Entities;

namespace Xpense.Persistence.TypeConfiguration;

public class NotificationEntityTypeConfiguration : BaseEntityTypeConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);
        builder.Metadata.SetSchema(XpenseSchema);

        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Message).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.Payload).HasColumnType("jsonb").IsRequired();

        // A SHA-256 in hex is exactly 64 characters, so the column says so.
        builder.Property(e => e.PayloadHash).HasMaxLength(64).IsRequired().IsFixedLength();

        // The identity of a notification: one event may warrant several, but never two identical ones.
        // This is what makes a redelivered event insert nothing, and it holds even against a replay or
        // a second worker, which no amount of in-process checking would.
        builder.HasIndex(e => new { e.EventId, e.PayloadHash })
            .IsUnique()
            .HasDatabaseName("UQ_Notifications_Event_Payload");

        // Every read is newest-first, and the unread filter is the common one.
        builder.HasIndex(e => e.CreatedAt).IsDescending();
        builder.HasIndex(e => e.ReadAt).HasFilter("\"ReadAt\" IS NULL");

        // No foreign key on OwnerId: there is no User table to point at. Stated here so the absence
        // reads as deliberate rather than forgotten.
    }
}
