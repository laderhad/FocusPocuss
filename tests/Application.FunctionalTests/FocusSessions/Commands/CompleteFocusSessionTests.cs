using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.FocusSessions.Commands.CompleteFocusSession;
using FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.FunctionalTests.FocusSessions.Commands;

public class CompleteFocusSessionTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(
            () => TestApp.SendAsync(new CompleteFocusSessionCommand(1)));
    }

    [Test]
    public async Task ShouldCompleteOwnedSessionOnlyAfterExplicitCommand()
    {
        await TestApp.RunAsDefaultUserAsync();
        var session = await StartSessionAsync("Prepare the release notes");

        session.CompletedAtUtc.ShouldBeNull();
        (await TestApp.FindAsync<FocusSession>(session.Id))!.CompletedAtUtc.ShouldBeNull();

        var completed = await TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id));
        var completedAgain = await TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id));

        completed.CompletedAtUtc.ShouldNotBeNull();
        completed.CompletedAtUtc.Value.ShouldBeGreaterThanOrEqualTo(session.StartedAtUtc);
        completedAgain.CompletedAtUtc.ShouldNotBeNull();
        completedAgain.CompletedAtUtc.Value.ShouldBe(
            completed.CompletedAtUtc.Value,
            DatabaseTimestampPrecision);
        var persistedCompletedAtUtc = (await TestApp.FindAsync<FocusSession>(session.Id))!.CompletedAtUtc;
        persistedCompletedAtUtc.ShouldNotBeNull();
        persistedCompletedAtUtc.Value.ShouldBe(
            completed.CompletedAtUtc.Value,
            DatabaseTimestampPrecision);
    }

    [Test]
    public async Task ShouldNotCompleteAnotherUsersSession()
    {
        await TestApp.RunAsUserAsync("owner@local", "Testing1234!", []);
        var session = await StartSessionAsync("Owner's task");

        await TestApp.RunAsDefaultUserAsync();

        await Should.ThrowAsync<NotFoundException>(
            () => TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id)));

        (await TestApp.FindAsync<FocusSession>(session.Id))!.CompletedAtUtc.ShouldBeNull();
    }

    private static async Task<FocusPocuss.Application.FocusSessions.FocusSessionDto> StartSessionAsync(
        string originalInput)
    {
        var task = await TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = originalInput
        });
        var plan = await TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en"));

        return await TestApp.SendAsync(new StartFocusSessionCommand(task.Id, plan.Id));
    }
}
