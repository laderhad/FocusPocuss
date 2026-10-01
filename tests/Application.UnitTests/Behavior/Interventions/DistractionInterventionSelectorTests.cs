using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace FocusPocuss.Application.UnitTests.Behavior.Interventions;

public class DistractionInterventionSelectorTests
{
    private readonly DistractionInterventionSelector _selector = new();

    [TestCase(DistractionReason.TaskTooDifficult, RecoveryInterventionType.ShrinkCurrentAction,
        RecoveryRequirement.ActionTransformation)]
    [TestCase(DistractionReason.UnclearNextAction, RecoveryInterventionType.ClarifyCurrentAction,
        RecoveryRequirement.ActionTransformation)]
    [TestCase(DistractionReason.PhoneOrSocialMedia, RecoveryInterventionType.EnvironmentalReset,
        RecoveryRequirement.None)]
    [TestCase(DistractionReason.AnotherThought, RecoveryInterventionType.ParkThought,
        RecoveryRequirement.ThoughtCapture)]
    [TestCase(DistractionReason.Tired, RecoveryInterventionType.RecoveryChoice,
        RecoveryRequirement.UserChoice)]
    [TestCase(DistractionReason.Other, RecoveryInterventionType.ReconnectToCurrentAction,
        RecoveryRequirement.None)]
    public void ShouldSelectDistinctVersionedIntervention(
        DistractionReason reason, RecoveryInterventionType type, RecoveryRequirement requirement)
    {
        var session = Session("Write the first solution step.");

        var result = _selector.Select(session, reason);

        result.Type.ShouldBe(type);
        result.Requirement.ShouldBe(requirement);
        result.Version.ShouldBe("distraction-recovery-v1");
        result.CurrentAction.ShouldBe(session.Action);
        session.Action.ShouldBe("Write the first solution step.");
        session.CompletedAtUtc.ShouldBeNull();
    }

    [TestCase(DistractionReason.TaskTooDifficult)]
    [TestCase(DistractionReason.UnclearNextAction)]
    public void ShouldRequireTransformationWithoutPretendingToProduceAnAction(DistractionReason reason)
    {
        var result = _selector.Select(Session("Work on the internship report."), reason);

        result.Requirement.ShouldBe(RecoveryRequirement.ActionTransformation);
        result.ReturnAction.ShouldBeNull();
        result.Choices.ShouldBeEmpty();
    }

    [TestCase("Staj raporundaki ilk boş alanı, elindeki bilgilerle doldur.")]
    [TestCase("Fill the first empty field in the report using information you have.")]
    public void ShouldPreserveTheActualActionAcrossLanguages(string action)
    {
        foreach (var reason in new[]
        {
            DistractionReason.PhoneOrSocialMedia, DistractionReason.AnotherThought, DistractionReason.Other
        })
        {
            var result = _selector.Select(Session(action), reason);

            result.CurrentAction.ShouldBe(action);
            result.ReturnAction.ShouldBe(action);
            result.Choices.ShouldBeEmpty();
        }
    }

    [Test]
    public void ShouldOfferTiredUserAChoiceIncludingEndingTheSession()
    {
        var session = Session("Write the first solution step.");

        var result = _selector.Select(session, DistractionReason.Tired);

        result.Requirement.ShouldBe(RecoveryRequirement.UserChoice);
        result.ReturnAction.ShouldBe(session.Action);
        result.Choices.ShouldBe(new[]
        {
            RecoveryChoice.TakeShortReset,
            RecoveryChoice.ContinueWithSmallerAction,
            RecoveryChoice.EndSession
        });
        session.CompletedAtUtc.ShouldBeNull();
    }

    [Test]
    public void ShouldRejectUnsupportedReason()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            _selector.Select(Session("Write the first sentence."), (DistractionReason)999));
    }

    private static FocusSession Session(string action)
        => new("user", 1, 2, action, 10, DateTimeOffset.UtcNow);
}
