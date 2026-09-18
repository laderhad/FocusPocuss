namespace FocusPocuss.Application.Tasks;

public sealed class TaskDetailsDto
{
    public int Id { get; init; }

    public required string OriginalInput { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public TaskStartPlanDto? StartPlan { get; init; }
}
