using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace FocusPocuss.Domain.UnitTests.Entities;

public class RecoveryExecutionTests
{
    [Test]
    public void ShouldPreserveOriginalActionWhenApplyingRecovery()
    {
        var session = Session();
        session.ApplyRecoveryAction("Write the given values.");
        session.Action.ShouldBe("Solve the question.");
        session.CurrentAction.ShouldBe("Write the given values.");
        session.RecoveryRevision.ShouldBe(1);
    }

    [Test]
    public void ShouldKeepEarlyEndSeparateFromSuccessfulCompletion()
    {
        var session = Session();
        session.EndEarly(DateTimeOffset.UtcNow);
        session.CompletedAtUtc.ShouldBeNull();
        Should.Throw<InvalidOperationException>(() => session.Complete(DateTimeOffset.UtcNow));
        Should.Throw<InvalidOperationException>(() => session.ApplyRecoveryAction("Changed"));
        Should.Throw<InvalidOperationException>(() => session.RecordReflection(FocusSessionReflection.FocusedWell, DateTimeOffset.UtcNow));
    }

    [Test]
    public void ShouldRequireThoughtBeforeReturning()
    {
        var recovery = Recovery(RecoveryInterventionType.ParkThought, RecoveryRequirement.ThoughtCapture);
        Should.Throw<ArgumentException>(() => recovery.Resolve(RecoveryResolution.ReturnToFocus, DateTimeOffset.UtcNow));
        recovery.Resolve(RecoveryResolution.ReturnToFocus, DateTimeOffset.UtcNow, "  Remember this.  ");
        recovery.ParkedThought.ShouldBe("Remember this.");
        Should.Throw<InvalidOperationException>(() => recovery.Resolve(RecoveryResolution.Dismiss, DateTimeOffset.UtcNow));
    }

    [Test]
    public void ShouldNotReturnBeforeTransformation()
    {
        var recovery = Recovery(RecoveryInterventionType.ShrinkCurrentAction, RecoveryRequirement.ActionTransformation);
        Should.Throw<InvalidOperationException>(() => recovery.Resolve(RecoveryResolution.ReturnToFocus, DateTimeOffset.UtcNow));
        recovery.Prepare("Write the given values.", RecoveryRequirement.None);
        recovery.Resolve(RecoveryResolution.ReturnToFocus, DateTimeOffset.UtcNow);
        recovery.ProposedAction.ShouldBe("Write the given values.");
    }

    [Test]
    public void ShouldLimitClarificationToOneInput()
    {
        var recovery = Recovery(RecoveryInterventionType.ClarifyCurrentAction, RecoveryRequirement.ActionTransformation);
        recovery.Prepare(null, RecoveryRequirement.NeedsClarification);
        recovery.UseClarification();
        Should.Throw<InvalidOperationException>(() => recovery.UseClarification());
    }

    private static FocusSession Session() => new("user", 1, 1, "Solve the question.", 10, DateTimeOffset.UtcNow);
    private static DistractionEvent Recovery(RecoveryInterventionType type, RecoveryRequirement requirement)
    {
        var recovery = new DistractionEvent(1, DistractionReason.Other, DateTimeOffset.UtcNow);
        recovery.BeginRecovery(type, "v1", "Solve the question.", "en", requirement);
        return recovery;
    }
}
