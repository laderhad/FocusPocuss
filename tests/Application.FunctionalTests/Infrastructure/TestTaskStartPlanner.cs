using FocusPocuss.Application.Tasks.Planning;

namespace FocusPocuss.Application.FunctionalTests.Infrastructure;

public sealed class TestTaskStartPlanner : ITaskStartPlanner
{
    public TaskStartPlanResult Result { get; set; } = new(
        "You can make a small start.",
        "Open the task and identify the first concrete action.",
        8,
        "test-model",
        "test-prompt-v1");

    public Exception? ExceptionToThrow { get; set; }

    public List<TaskStartPlanningContext> Requests { get; } = [];

    public Task<TaskStartPlanResult> PlanAsync(
        TaskStartPlanningContext context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Requests.Add(context);

        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }

        return Task.FromResult(Result);
    }

    public void Reset()
    {
        Result = new TaskStartPlanResult(
            "You can make a small start.",
            "Open the task and identify the first concrete action.",
            8,
            "test-model",
            "test-prompt-v1");
        ExceptionToThrow = null;
        Requests.Clear();
    }
}
