using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.Behavior.Interventions;

public sealed class RecoveryActionPreparation(IRecoveryActionPlanner planner)
{
    public async Task PrepareAsync(DistractionEvent recovery, string? clarification, CancellationToken cancellationToken)
    {
        var type = recovery.InterventionType == RecoveryInterventionType.ClarifyCurrentAction
            ? RecoveryInterventionType.ClarifyCurrentAction : RecoveryInterventionType.ShrinkCurrentAction;
        var result = await planner.TransformAsync(new(recovery.ActionAtDistraction!, type,
            recovery.Language!, clarification), cancellationToken);
        // Application validation remains authoritative even for another provider implementation.
        if (result.Status == RecoveryActionStatus.Ready && !string.IsNullOrWhiteSpace(result.Action)
            && result.Action.Trim().Length <= 500 && !string.Equals(result.Action.Trim(),
                recovery.ActionAtDistraction!.Trim(), StringComparison.OrdinalIgnoreCase))
            recovery.Prepare(result.Action, RecoveryRequirement.None);
        else
            recovery.Prepare(null, result.Status == RecoveryActionStatus.NeedsClarification && !recovery.ClarificationUsed
                ? RecoveryRequirement.NeedsClarification : RecoveryRequirement.Unavailable);
    }
}
