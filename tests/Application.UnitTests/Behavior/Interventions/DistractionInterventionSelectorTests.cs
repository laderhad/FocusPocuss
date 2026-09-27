using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace FocusPocuss.Application.UnitTests.Behavior.Interventions;

public class DistractionInterventionSelectorTests
{
    private readonly DistractionInterventionSelector _selector = new();

    [TestCase(
        DistractionReason.TaskTooDifficult,
        InterventionStrategy.TaskDecomposition)]
    [TestCase(
        DistractionReason.UnclearNextAction,
        InterventionStrategy.ClarifyNextAction)]
    [TestCase(
        DistractionReason.PhoneOrSocialMedia,
        InterventionStrategy.RemoveFriction)]
    [TestCase(
        DistractionReason.AnotherThought,
        InterventionStrategy.DistractionRecovery)]
    [TestCase(
        DistractionReason.Tired,
        InterventionStrategy.BreakRecommendation)]
    [TestCase(
        DistractionReason.Other,
        InterventionStrategy.DistractionRecovery)]
    public void ShouldSelectApprovedStrategy(
        DistractionReason reason,
        InterventionStrategy expectedStrategy)
    {
        _selector.Select(reason).ShouldBe(expectedStrategy);
    }

    [Test]
    public void ShouldRejectUnsupportedReason()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            _selector.Select((DistractionReason)999));
    }
}
