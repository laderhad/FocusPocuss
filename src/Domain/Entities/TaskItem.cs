namespace FocusPocuss.Domain.Entities;

public class TaskItem : BaseAuditableEntity
{
    private TaskItem()
    {
    }

    public TaskItem(string userId, string originalInput)
    {
        UserId = userId;
        OriginalInput = originalInput;
    }

    public string UserId { get; private set; } = string.Empty;

    public string OriginalInput { get; private set; } = string.Empty;
}
