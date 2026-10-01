using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.Behavior.Interventions;

public interface IRecoveryActionPlanner
{
    Task<RecoveryActionResult> TransformAsync(RecoveryActionContext context, CancellationToken cancellationToken);
}

public sealed record RecoveryActionContext(
    string CurrentAction, RecoveryInterventionType Transformation, string Language, string? Clarification = null);

public enum RecoveryActionStatus { Ready, NeedsClarification, Unavailable }

public sealed record RecoveryActionResult(RecoveryActionStatus Status, string? Action = null);
