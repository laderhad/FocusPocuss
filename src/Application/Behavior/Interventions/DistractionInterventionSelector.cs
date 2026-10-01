using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.Behavior.Interventions;

public sealed class DistractionInterventionSelector
{
    public const string Version = "distraction-recovery-v1";

    public RecoveryIntervention Select(FocusSession session, DistractionReason reason)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(session.CurrentAction);

        var (type, requirement) = reason switch
        {
            DistractionReason.TaskTooDifficult =>
                (RecoveryInterventionType.ShrinkCurrentAction, RecoveryRequirement.ActionTransformation),
            DistractionReason.UnclearNextAction =>
                (RecoveryInterventionType.ClarifyCurrentAction, RecoveryRequirement.ActionTransformation),
            DistractionReason.PhoneOrSocialMedia =>
                (RecoveryInterventionType.EnvironmentalReset, RecoveryRequirement.None),
            DistractionReason.AnotherThought =>
                (RecoveryInterventionType.ParkThought, RecoveryRequirement.ThoughtCapture),
            DistractionReason.Tired =>
                (RecoveryInterventionType.RecoveryChoice, RecoveryRequirement.UserChoice),
            DistractionReason.Other =>
                (RecoveryInterventionType.ReconnectToCurrentAction, RecoveryRequirement.None),
            _ => throw new ArgumentOutOfRangeException(
                nameof(reason),
                reason,
                "Unsupported distraction reason.")
        };

        return new RecoveryIntervention(
            type,
            Version,
            session.CurrentAction,
            requirement == RecoveryRequirement.ActionTransformation ? null : session.CurrentAction,
            requirement,
            type == RecoveryInterventionType.RecoveryChoice
                ? Array.AsReadOnly(new[]
                {
                    RecoveryChoice.TakeShortReset,
                    RecoveryChoice.ContinueWithSmallerAction,
                    RecoveryChoice.EndSession
                })
                : Array.Empty<RecoveryChoice>());
    }
}
