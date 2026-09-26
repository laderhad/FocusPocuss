using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;
using FocusPocuss.Application.Tasks;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FocusPocuss.Application.FunctionalTests.FocusSessions.Commands;

public class StartFocusSessionTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(
            () => TestApp.SendAsync(new StartFocusSessionCommand(1, 1)));
    }

    [Test]
    public async Task ShouldPersistSessionFromOwnedStartPlan()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();
        var (task, plan) = await CreateTaskAndPlanAsync("Prepare the release notes");

        var result = await TestApp.SendAsync(new StartFocusSessionCommand(task.Id, plan.Id));

        var entity = await TestApp.FindAsync<FocusSession>(result.Id);
        entity.ShouldNotBeNull();
        entity.UserId.ShouldBe(userId);
        entity.TaskItemId.ShouldBe(task.Id);
        entity.TaskStartPlanId.ShouldBe(plan.Id);
        entity.Action.ShouldBe(plan.NextAction);
        entity.PlannedDurationMinutes.ShouldBe(plan.SuggestedDurationMinutes);
        entity.CompletedAtUtc.ShouldBeNull();

        result.Action.ShouldBe(plan.NextAction);
        result.PlannedDurationMinutes.ShouldBe(plan.SuggestedDurationMinutes);
        result.CompletedAtUtc.ShouldBeNull();
    }

    [Test]
    public async Task ShouldReturnExistingActiveSessionWhenAnotherOwnedTaskIsStarted()
    {
        await TestApp.RunAsDefaultUserAsync();
        var first = await CreateTaskAndPlanAsync("Prepare the release notes");
        var second = await CreateTaskAndPlanAsync("Review the launch checklist");

        var existing = await TestApp.SendAsync(
            new StartFocusSessionCommand(first.Task.Id, first.Plan.Id));
        var result = await TestApp.SendAsync(
            new StartFocusSessionCommand(second.Task.Id, second.Plan.Id));

        result.Id.ShouldBe(existing.Id);
        result.TaskId.ShouldBe(first.Task.Id);
        result.TaskStartPlanId.ShouldBe(first.Plan.Id);
        (await TestApp.CountAsync<FocusSession>()).ShouldBe(1);
    }

    [Test]
    public async Task ShouldValidateRequestedOwnershipBeforeReturningActiveSession()
    {
        await TestApp.RunAsUserAsync("other@local", "Testing1234!", []);
        var otherUsersTask = await CreateTaskAndPlanAsync("Other user's task");

        await TestApp.RunAsDefaultUserAsync();
        var ownTask = await CreateTaskAndPlanAsync("My task");
        var activeSession = await TestApp.SendAsync(
            new StartFocusSessionCommand(ownTask.Task.Id, ownTask.Plan.Id));

        await Should.ThrowAsync<NotFoundException>(
            () => TestApp.SendAsync(
                new StartFocusSessionCommand(
                    otherUsersTask.Task.Id,
                    otherUsersTask.Plan.Id)));

        (await TestApp.CountAsync<FocusSession>()).ShouldBe(1);
        (await TestApp.FindAsync<FocusSession>(activeSession.Id)).ShouldNotBeNull();
    }

    [Test]
    public async Task ShouldEnforceOneIncompleteSessionPerUserInPostgreSql()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();
        var (task, plan) = await CreateTaskAndPlanAsync("Prepare the release notes");

        var first = new FocusSession(
            userId,
            task.Id,
            plan.Id,
            plan.NextAction,
            plan.SuggestedDurationMinutes,
            DateTimeOffset.UtcNow);
        var second = new FocusSession(
            userId,
            task.Id,
            plan.Id,
            plan.NextAction,
            plan.SuggestedDurationMinutes,
            DateTimeOffset.UtcNow);

        await TestApp.AddAsync(first);

        await Should.ThrowAsync<DbUpdateException>(() => TestApp.AddAsync(second));
        (await TestApp.CountAsync<FocusSession>()).ShouldBe(1);
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
