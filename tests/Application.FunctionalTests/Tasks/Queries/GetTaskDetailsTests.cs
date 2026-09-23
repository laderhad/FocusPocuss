using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Application.Tasks.Queries.GetTaskDetails;

namespace FocusPocuss.Application.FunctionalTests.Tasks.Queries;

public class GetTaskDetailsTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(
            () => TestApp.SendAsync(new GetTaskDetailsQuery(1)));
    }

    [Test]
    public async Task ShouldNotReturnAnotherUsersTask()
    {
        await TestApp.RunAsUserAsync("first@local", "Testing1234!", []);
        var task = await CreateTaskAsync("First user's task");

        await TestApp.RunAsUserAsync("second@local", "Testing1234!", []);

        await Should.ThrowAsync<NotFoundException>(
            () => TestApp.SendAsync(new GetTaskDetailsQuery(task.Id)));
    }

    [Test]
    public async Task ShouldReturnExactOriginalInputWithoutPlan()
    {
        await TestApp.RunAsDefaultUserAsync();
        const string originalInput = "  Prepare the project brief\nwith the original spacing.  ";
        var task = await CreateTaskAsync(originalInput);

        var result = await TestApp.SendAsync(new GetTaskDetailsQuery(task.Id));

        result.Id.ShouldBe(task.Id);
        result.OriginalInput.ShouldBe(originalInput);
        result.StartPlan.ShouldBeNull();
    }

    [Test]
    public async Task ShouldReturnPersistedStartPlan()
    {
        await TestApp.RunAsDefaultUserAsync();
        var task = await CreateTaskAsync("Prepare the project brief");
        var plan = await TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en"));

        var result = await TestApp.SendAsync(new GetTaskDetailsQuery(task.Id));

        result.StartPlan.ShouldNotBeNull();
        result.StartPlan.Id.ShouldBe(plan.Id);
        result.StartPlan.Message.ShouldBe(plan.Message);
        result.StartPlan.NextAction.ShouldBe(plan.NextAction);
        result.StartPlan.SuggestedDurationMinutes.ShouldBe(plan.SuggestedDurationMinutes);
    }

    private static Task<FocusPocuss.Application.Tasks.TaskDto> CreateTaskAsync(string originalInput)
    {
        return TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = originalInput
        });
    }
}
