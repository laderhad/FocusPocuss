namespace FocusPocuss.Domain.Entities;

public class TaskStartPlan : BaseAuditableEntity
{
    private TaskStartPlan()
    {
    }

    public TaskStartPlan(
        int taskItemId,
        string message,
        string nextAction,
        int suggestedDurationMinutes,
        string language,
        string model,
        string promptVersion)
    {
        TaskItemId = taskItemId;
        Message = message;
        NextAction = nextAction;
        SuggestedDurationMinutes = suggestedDurationMinutes;
        Language = language;
        Model = model;
        PromptVersion = promptVersion;
    }

    public int TaskItemId { get; private set; }

    public string Message { get; private set; } = string.Empty;

    public string NextAction { get; private set; } = string.Empty;

    public int SuggestedDurationMinutes { get; private set; }

    public string Language { get; private set; } = string.Empty;

    public string Model { get; private set; } = string.Empty;

    public string PromptVersion { get; private set; } = string.Empty;

    public TaskItem TaskItem { get; private set; } = null!;
}
