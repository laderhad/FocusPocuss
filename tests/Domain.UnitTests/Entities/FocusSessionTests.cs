using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace FocusPocuss.Domain.UnitTests.Entities;

public class FocusSessionTests
{
    [Test]
    public void ShouldRecordReflectionForCompletedSessionAndNormalizeTimestampToUtc()
    {
        var session = CreateSession();
        session.Complete(new DateTimeOffset(2026, 9, 28, 12, 0, 0, TimeSpan.Zero));
        var reflectedAt = new DateTimeOffset(2026, 9, 28, 15, 5, 0, TimeSpan.FromHours(3));

        session.RecordReflection(FocusSessionReflection.FocusedWell, reflectedAt);

        session.Reflection.ShouldBe(FocusSessionReflection.FocusedWell);
        session.ReflectedAtUtc.ShouldBe(reflectedAt.ToUniversalTime());
        session.ReflectedAtUtc!.Value.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Test]
    public void ShouldUpdateExistingReflection()
    {
        var session = CreateSession();
        session.Complete(new DateTimeOffset(2026, 9, 28, 12, 0, 0, TimeSpan.Zero));
        var updatedAt = new DateTimeOffset(2026, 9, 28, 12, 10, 0, TimeSpan.Zero);

        session.RecordReflection(
            FocusSessionReflection.FocusedWell,
            new DateTimeOffset(2026, 9, 28, 12, 5, 0, TimeSpan.Zero));
        session.RecordReflection(FocusSessionReflection.SomeDifficulty, updatedAt);

        session.Reflection.ShouldBe(FocusSessionReflection.SomeDifficulty);
        session.ReflectedAtUtc.ShouldBe(updatedAt);
    }

    [Test]
    public void ShouldRejectReflectionForActiveSession()
    {
        var session = CreateSession();

        Should.Throw<InvalidOperationException>(() =>
            session.RecordReflection(
                FocusSessionReflection.SignificantDifficulty,
                DateTimeOffset.UtcNow));

        session.Reflection.ShouldBeNull();
        session.ReflectedAtUtc.ShouldBeNull();
    }

    [Test]
    public void ShouldRejectUnsupportedReflection()
    {
        var session = CreateSession();
        session.Complete(DateTimeOffset.UtcNow);

        Should.Throw<ArgumentOutOfRangeException>(() =>
            session.RecordReflection(
                (FocusSessionReflection)999,
                DateTimeOffset.UtcNow));
    }

    private static FocusSession CreateSession()
    {
        return new FocusSession(
            "user-id",
            1,
            1,
            "Open the project and review the first endpoint.",
            10,
            new DateTimeOffset(2026, 9, 28, 11, 45, 0, TimeSpan.Zero));
    }
}
