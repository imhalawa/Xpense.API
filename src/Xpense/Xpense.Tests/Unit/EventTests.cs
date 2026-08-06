using FluentAssertions;
using Xpense.Domain.Enums;
using Xpense.Domain.Events;

namespace Xpense.Tests.Unit;

[TestFixture]
public class EventTests
{
    private static readonly DateTime OccurredAt = new(2026, 8, 6, 9, 30, 0, DateTimeKind.Utc);

    [Test]
    public void An_event_is_stamped_with_the_body_type_name()
    {
        var @event = Event.Of(Body());

        // The type name is how a stored row is routed back to a body to deserialize into, so renaming
        // a body type breaks anything unprocessed. Asserted so that rename shows up here first.
        @event.Attributes.Type.Should().Be(nameof(TransactionRecorded));
        @event.Attributes.Version.Should().Be(Event.CurrentVersion);
        @event.Attributes.Source.Should().Be(Event.DefaultSource);
    }

    [Test]
    public void Every_event_gets_its_own_identity()
    {
        var first = Event.Of(Body());
        var second = Event.Of(Body());

        first.Attributes.EventId.Should().NotBe(second.Attributes.EventId);
        first.Attributes.EventId.Should().NotBe(Guid.Empty);
    }

    /// <summary>
    /// Version 7 GUIDs are time-ordered, which is why they were chosen: the index on EventId stays
    /// dense as rows arrive instead of scattering inserts across it. Asserting the version keeps a
    /// well-meaning switch to Guid.NewGuid from silently undoing that.
    /// <para>
    /// Ordering is checked on the textual form, not with <c>Guid.CompareTo</c>. .NET compares the
    /// first four bytes as a *signed* little-endian integer, so its ordering does not follow the
    /// bytes and two version 7 GUIDs can compare backwards. The byte order is what is actually
    /// time-sorted, and it is what Postgres uses to compare a <c>uuid</c> -- which is the property
    /// the index benefits from.
    /// </para>
    /// </summary>
    /// <para>
    /// The two ids are deliberately separated in time. A version 7 GUID encodes a *millisecond*
    /// timestamp and fills the rest with randomness, so two created within the same millisecond have
    /// no defined order between them -- asserting otherwise is a test that passes until the machine is
    /// fast enough to break it.
    /// </para>
    [Test]
    public async Task Event_ids_are_version_7_and_therefore_sort_by_creation()
    {
        var first = Event.Of(Body()).Attributes.EventId;
        await Task.Delay(5);
        var second = Event.Of(Body()).Attributes.EventId;

        Version(first).Should().Be(7);
        Version(second).Should().Be(7);
        string.CompareOrdinal(second.ToString(), first.ToString())
            .Should().BePositive("a later event should sort after an earlier one");
    }

    /// <summary>
    /// When it happened is not always now: a backdated transaction happened when the money moved, and
    /// reporting uses that. Defaulting to now is right for most events and wrong for this one.
    /// </summary>
    [Test]
    public void An_event_can_say_when_it_happened_rather_than_now()
    {
        Event.Of(Body(), OccurredAt).Attributes.OccurredAt.Should().Be(OccurredAt);
    }

    [Test]
    public void An_event_defaults_to_happening_now()
    {
        var before = DateTime.UtcNow;

        var @event = Event.Of(Body());

        @event.Attributes.OccurredAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    private static TransactionRecorded Body() => new(
        TransactionId: 1,
        Kind: TransactionKind.Expense,
        AmountMinorUnits: 1250,
        Currency: Currency.EUR,
        OccurredAt: OccurredAt,
        CategoryId: 3,
        MerchantId: 2,
        SourceAccountNumber: "1000000000",
        SourceBalanceAfterMinorUnits: 8750,
        DestinationAccountNumber: null,
        DestinationBalanceAfterMinorUnits: null);

    /// <summary>The version nibble of a GUID, per RFC 9562.</summary>
    private static int Version(Guid value) => (value.ToByteArray()[7] & 0xF0) >> 4;
}
