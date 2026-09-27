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

    public InterventionStrategy Strategy { get; init; }

    internal static DistractionReportDto FromEntity(
        DistractionEvent distractionEvent,
        InterventionStrategy strategy)
    {
        return new DistractionReportDto
        {
            Id = distractionEvent.Id,
            FocusSessionId = distractionEvent.FocusSessionId,
            Reason = distractionEvent.Reason,
            OccurredAtUtc = distractionEvent.OccurredAtUtc,
            Strategy = strategy
        };
    }
}
