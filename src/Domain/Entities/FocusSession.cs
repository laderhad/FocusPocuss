namespace FocusPocuss.Domain.Entities;

public class FocusSession : BaseAuditableEntity
{
    private readonly List<DistractionEvent> _distractionEvents = [];

    private FocusSession()
    {
    }

    public FocusSession(
        string userId,
        int taskItemId,
        int taskStartPlanId,
        string action,
        int plannedDurationMinutes,
        DateTimeOffset startedAtUtc)
    {
        UserId = userId;
        TaskItemId = taskItemId;
        TaskStartPlanId = taskStartPlanId;
        Action = action;
        PlannedDurationMinutes = plannedDurationMinutes;
        StartedAtUtc = startedAtUtc;
    }

    public string UserId { get; private set; } = string.Empty;

    public int TaskItemId { get; private set; }

    public int TaskStartPlanId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? RecoveryAction { get; private set; }

    public string CurrentAction => RecoveryAction ?? Action;

    public int RecoveryRevision { get; private set; }

    public DateTimeOffset? EndedEarlyAtUtc { get; private set; }

    public int PlannedDurationMinutes { get; private set; }

    public DateTimeOffset StartedAtUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public FocusSessionReflection? Reflection { get; private set; }

    public DateTimeOffset? ReflectedAtUtc { get; private set; }

    public TaskItem TaskItem { get; private set; } = null!;

    public TaskStartPlan TaskStartPlan { get; private set; } = null!;

    public IReadOnlyCollection<DistractionEvent> DistractionEvents => _distractionEvents.AsReadOnly();

    public void Complete(DateTimeOffset completedAtUtc)
    {
        if (CompletedAtUtc is not null) return;
        TouchRecovery();
        CompletedAtUtc = completedAtUtc.ToUniversalTime();
    }

    public void TouchRecovery()
    {
        if (CompletedAtUtc is not null || EndedEarlyAtUtc is not null)
            throw new InvalidOperationException("The focus session has ended.");
        RecoveryRevision++;
    }

    public void ApplyRecoveryAction(string action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        if (action.Trim().Length > 500) throw new ArgumentOutOfRangeException(nameof(action));
        TouchRecovery();
        RecoveryAction = action.Trim();
    }

    public void EndEarly(DateTimeOffset endedAtUtc)
    {
        TouchRecovery();
        EndedEarlyAtUtc = endedAtUtc.ToUniversalTime();
    }

    public void RecordReflection(
        FocusSessionReflection reflection,
        DateTimeOffset reflectedAtUtc)
    {
        if (CompletedAtUtc is null)
        {
            throw new InvalidOperationException(
                "A reflection can only be recorded for a completed focus session.");
        }

        if (!Enum.IsDefined(reflection))
        {
            throw new ArgumentOutOfRangeException(
                nameof(reflection),
                reflection,
                "Unsupported focus session reflection.");
        }

        Reflection = reflection;
        ReflectedAtUtc = reflectedAtUtc.ToUniversalTime();
    }
}
