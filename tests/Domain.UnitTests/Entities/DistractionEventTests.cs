using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace FocusPocuss.Domain.UnitTests.Entities;

public class DistractionEventTests
{
    [Test]
    public void ShouldStoreReasonAndNormalizeTimestampToUtc()
    {
        var occurredAt = new DateTimeOffset(2026, 9, 27, 15, 30, 0, TimeSpan.FromHours(3));

        var distraction = new DistractionEvent(
            42,
            DistractionReason.UnclearNextAction,
            occurredAt);

        distraction.FocusSessionId.ShouldBe(42);
        distraction.Reason.ShouldBe(DistractionReason.UnclearNextAction);
        distraction.OccurredAtUtc.ShouldBe(occurredAt.ToUniversalTime());
        distraction.OccurredAtUtc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void ShouldRejectUnsupportedReason()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            new DistractionEvent(42, (DistractionReason)999, DateTimeOffset.UtcNow));
    }
}
