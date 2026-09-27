using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions;

public sealed class DistractionEventDto
{
    public int Id { get; init; }

    public int FocusSessionId { get; init; }

    public DistractionReason Reason { get; init; }

    public DateTimeOffset OccurredAtUtc { get; init; }

    internal static DistractionEventDto FromEntity(DistractionEvent distractionEvent)
    {
        return new DistractionEventDto
        {
            Id = distractionEvent.Id,
            FocusSessionId = distractionEvent.FocusSessionId,
            Reason = distractionEvent.Reason,
            OccurredAtUtc = distractionEvent.OccurredAtUtc
        };
    }
}
