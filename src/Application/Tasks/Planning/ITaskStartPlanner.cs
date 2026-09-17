namespace FocusPocuss.Application.Tasks.Planning;

public interface ITaskStartPlanner
{
    Task<TaskStartPlanResult> PlanAsync(
        TaskStartPlanningContext context,
        CancellationToken cancellationToken);
}

public sealed record TaskStartPlanningContext(
    string OriginalInput,
    string Language);

public sealed record TaskStartPlanResult(
    string Message,
    string NextAction,
    int SuggestedDurationMinutes,
    string Model,
    string PromptVersion);
