using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.Behavior.Interventions;

public sealed class DistractionInterventionSelector
{
    public InterventionStrategy Select(DistractionReason reason)
    {
        return reason switch
        {
            DistractionReason.TaskTooDifficult => InterventionStrategy.TaskDecomposition,
            DistractionReason.UnclearNextAction => InterventionStrategy.ClarifyNextAction,
            DistractionReason.PhoneOrSocialMedia => InterventionStrategy.RemoveFriction,
            DistractionReason.AnotherThought => InterventionStrategy.DistractionRecovery,
            DistractionReason.Tired => InterventionStrategy.BreakRecommendation,
            DistractionReason.Other => InterventionStrategy.DistractionRecovery,
            _ => throw new ArgumentOutOfRangeException(
                nameof(reason),
                reason,
                "Unsupported distraction reason.")
        };
    }
}
