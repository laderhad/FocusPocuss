namespace FocusPocuss.Application.Tasks;

public sealed class TaskStartPlanDto
{
    public int Id { get; init; }

    public required string Message { get; init; }

    public required string NextAction { get; init; }

    public int SuggestedDurationMinutes { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
