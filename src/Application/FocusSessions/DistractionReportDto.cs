using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions;

public sealed class DistractionReportDto
{
    public int Id { get; init; }

    public int FocusSessionId { get; init; }

    public DistractionReason Reason { get; init; }

    public DateTimeOffset OccurredAtUtc { get; init; }

    // Compatibility for the existing UI; new recovery consumers should use Intervention.
    public InterventionStrategy Strategy { get; init; }

    public bool ClarificationUsed { get; init; }

    public RecoveryChoice? Choice { get; init; }

    public required RecoveryIntervention Intervention { get; init; }

    internal static DistractionReportDto FromEntity(
        DistractionEvent distractionEvent)
    {
        var intervention = new RecoveryIntervention(
            distractionEvent.InterventionType!.Value, distractionEvent.StrategyVersion!,
            distractionEvent.ActionAtDistraction!, distractionEvent.ProposedAction,
            distractionEvent.Requirement!.Value,
            distractionEvent.Requirement == RecoveryRequirement.UserChoice
                ? new[] { RecoveryChoice.TakeShortReset, RecoveryChoice.ContinueWithSmallerAction, RecoveryChoice.EndSession }
                : Array.Empty<RecoveryChoice>());
        return new DistractionReportDto
        {
            Id = distractionEvent.Id,
            FocusSessionId = distractionEvent.FocusSessionId,
            Reason = distractionEvent.Reason,
            OccurredAtUtc = distractionEvent.OccurredAtUtc,
            Strategy = intervention.Type switch
            {
                RecoveryInterventionType.ShrinkCurrentAction => InterventionStrategy.TaskDecomposition,
                RecoveryInterventionType.ClarifyCurrentAction => InterventionStrategy.ClarifyNextAction,
                RecoveryInterventionType.EnvironmentalReset => InterventionStrategy.RemoveFriction,
                RecoveryInterventionType.ParkThought => InterventionStrategy.DistractionRecovery,
                RecoveryInterventionType.RecoveryChoice => InterventionStrategy.BreakRecommendation,
                RecoveryInterventionType.ReconnectToCurrentAction => InterventionStrategy.DistractionRecovery,
                _ => throw new ArgumentOutOfRangeException(nameof(intervention))
            },
            ClarificationUsed = distractionEvent.ClarificationUsed,
            Choice = distractionEvent.Choice,
            Intervention = intervention
        };
    }
}
