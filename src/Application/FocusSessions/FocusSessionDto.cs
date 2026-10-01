using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FocusSessions;

public sealed class FocusSessionDto
{
    public int Id { get; init; }

    public int TaskId { get; init; }

    public int TaskStartPlanId { get; init; }

    public required string Action { get; init; }

    public string? OriginalAction { get; init; }

    public DateTimeOffset? EndedEarlyAtUtc { get; init; }

    public DistractionReportDto? PendingRecovery { get; init; }

    public IReadOnlyList<ParkedThoughtDto> ParkedThoughts { get; init; } = [];

    public int PlannedDurationMinutes { get; init; }

    public DateTimeOffset StartedAtUtc { get; init; }

    public DateTimeOffset? CompletedAtUtc { get; init; }

    public FocusSessionReflection? Reflection { get; init; }

    public DateTimeOffset? ReflectedAtUtc { get; init; }

    internal static FocusSessionDto FromEntity(FocusSession session)
    {
        return new FocusSessionDto
        {
            Id = session.Id,
            TaskId = session.TaskItemId,
            TaskStartPlanId = session.TaskStartPlanId,
            Action = session.CurrentAction,
            OriginalAction = session.Action,
            EndedEarlyAtUtc = session.EndedEarlyAtUtc,
            PendingRecovery = session.CompletedAtUtc is null && session.EndedEarlyAtUtc is null
                ? session.DistractionEvents.Where(item => item.InterventionType != null && item.ResolvedAtUtc == null)
                    .Select(DistractionReportDto.FromEntity).SingleOrDefault() : null,
            ParkedThoughts = session.DistractionEvents.Where(item => item.ParkedThought != null)
                .OrderBy(item => item.OccurredAtUtc)
                .Select(item => new ParkedThoughtDto(item.Id, item.ParkedThought!, item.ResolvedAtUtc!.Value)).ToArray(),
            PlannedDurationMinutes = session.PlannedDurationMinutes,
            StartedAtUtc = session.StartedAtUtc,
            CompletedAtUtc = session.CompletedAtUtc,
            Reflection = session.Reflection,
            ReflectedAtUtc = session.ReflectedAtUtc
        };
    }
}

public sealed record ParkedThoughtDto(int Id, string Text, DateTimeOffset SavedAtUtc);
