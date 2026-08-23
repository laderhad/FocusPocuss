namespace FocusPocuss.Application.Tasks;

public sealed class TaskDto
{
    public int Id { get; init; }

    public required string OriginalInput { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
