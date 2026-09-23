using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.FocusSessions;

public sealed class FocusSessionDto
{
    public int Id { get; init; }

    public int TaskId { get; init; }

    public int TaskStartPlanId { get; init; }

    public required string Action { get; init; }

    public int PlannedDurationMinutes { get; init; }

    public DateTimeOffset StartedAtUtc { get; init; }

    public DateTimeOffset? CompletedAtUtc { get; init; }

    internal static FocusSessionDto FromEntity(FocusSession session)
    {
        return new FocusSessionDto
        {
            Id = session.Id,
            TaskId = session.TaskItemId,
            TaskStartPlanId = session.TaskStartPlanId,
            Action = session.Action,
            PlannedDurationMinutes = session.PlannedDurationMinutes,
            StartedAtUtc = session.StartedAtUtc,
            CompletedAtUtc = session.CompletedAtUtc
        };
    }
}
