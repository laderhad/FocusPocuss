using FocusPocuss.Application.Behavior.Interventions;

namespace FocusPocuss.Application.FunctionalTests.Infrastructure;

public sealed class TestRecoveryActionPlanner : IRecoveryActionPlanner
{
    public RecoveryActionResult Result { get; set; } = new(RecoveryActionStatus.NeedsClarification);
    public List<RecoveryActionContext> Requests { get; } = [];

    public Task<RecoveryActionResult> TransformAsync(RecoveryActionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Requests.Add(context);
        return Task.FromResult(Result);
    }

    public void Reset()
    {
        Result = new(RecoveryActionStatus.NeedsClarification);
        Requests.Clear();
    }
}
