namespace FocusPocuss.Domain.Entities;

public class FocusSession : BaseAuditableEntity
{
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

    public int PlannedDurationMinutes { get; private set; }

    public DateTimeOffset StartedAtUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public TaskItem TaskItem { get; private set; } = null!;

    public TaskStartPlan TaskStartPlan { get; private set; } = null!;
}
