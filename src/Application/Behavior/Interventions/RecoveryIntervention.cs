using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.Behavior.Interventions;

// A known return action does not bypass an outstanding input or choice requirement.
public sealed record RecoveryIntervention(
    RecoveryInterventionType Type,
    string Version,
    string CurrentAction,
    string? ReturnAction,
    RecoveryRequirement Requirement,
    IReadOnlyList<RecoveryChoice> Choices);
