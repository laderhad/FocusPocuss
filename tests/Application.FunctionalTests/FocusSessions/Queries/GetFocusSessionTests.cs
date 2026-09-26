using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;
using FocusPocuss.Application.FocusSessions.Queries.GetFocusSession;
using FocusPocuss.Application.Tasks;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.FunctionalTests.FocusSessions.Queries;

public class GetFocusSessionTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(
            () => TestApp.SendAsync(new GetFocusSessionQuery(1)));
    }

    [Test]
    public async Task ShouldReturnExpiredIncompleteSessionWithoutCompletingIt()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();
        var (task, plan) = await CreateTaskAndPlanAsync("Prepare the release notes");
        var startedAtUtc = DateTimeOffset.UtcNow.AddHours(-1);
        var session = new FocusSession(
            userId,
            task.Id,
            plan.Id,
            plan.NextAction,
            10,
            startedAtUtc);
        await TestApp.AddAsync(session);

        var result = await TestApp.SendAsync(new GetFocusSessionQuery(session.Id));

        result.Id.ShouldBe(session.Id);
        result.StartedAtUtc.ShouldBe(startedAtUtc, DatabaseTimestampPrecision);
        result.CompletedAtUtc.ShouldBeNull();
        (await TestApp.FindAsync<FocusSession>(session.Id))!.CompletedAtUtc.ShouldBeNull();
    }

    [Test]
    public async Task ShouldNotReturnAnotherUsersSession()
    {
        await TestApp.RunAsUserAsync("owner@local", "Testing1234!", []);
        var (task, plan) = await CreateTaskAndPlanAsync("Owner's task");
        var session = await TestApp.SendAsync(new StartFocusSessionCommand(task.Id, plan.Id));

        await TestApp.RunAsDefaultUserAsync();

        await Should.ThrowAsync<NotFoundException>(
            () => TestApp.SendAsync(new GetFocusSessionQuery(session.Id)));
    }

    private static async Task<(TaskDto Task, TaskStartPlanDto Plan)> CreateTaskAndPlanAsync(
        string originalInput)
    {
        var task = await TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = originalInput
        });
        var plan = await TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en"));

        return (task, plan);
    }
}
