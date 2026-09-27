using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Application.FocusSessions;
using FocusPocuss.Application.FocusSessions.Commands.CompleteFocusSession;
using FocusPocuss.Application.FocusSessions.Commands.ReportDistraction;
using FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FunctionalTests.FocusSessions.Commands;

public class ReportDistractionTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            TestApp.SendAsync(new ReportDistractionCommand(
                1,
                DistractionReason.AnotherThought)));
    }

    [Test]
    public async Task ShouldRequireSupportedReason()
    {
        await TestApp.RunAsDefaultUserAsync();

        await Should.ThrowAsync<ValidationException>(() =>
            TestApp.SendAsync(new ReportDistractionCommand(
                1,
                (DistractionReason)999)));
    }

    [Test]
    public async Task ShouldPersistDistractionForOwnedActiveSession()
    {
        await TestApp.RunAsDefaultUserAsync();
        var session = await StartSessionAsync("Prepare the release notes");

        var result = await TestApp.SendAsync(new ReportDistractionCommand(
            session.Id,
            DistractionReason.UnclearNextAction));

        var entity = await TestApp.FindAsync<DistractionEvent>(result.Id);

        entity.ShouldNotBeNull();
        entity.FocusSessionId.ShouldBe(session.Id);
        entity.Reason.ShouldBe(DistractionReason.UnclearNextAction);
        entity.OccurredAtUtc.Offset.ShouldBe(TimeSpan.Zero);
        entity.OccurredAtUtc.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));

        result.FocusSessionId.ShouldBe(entity.FocusSessionId);
        result.Reason.ShouldBe(entity.Reason);
        result.OccurredAtUtc.ShouldBe(entity.OccurredAtUtc, DatabaseTimestampPrecision);
        result.Strategy.ShouldBe(InterventionStrategy.ClarifyNextAction);
    }

    [Test]
    public async Task ShouldNotReportDistractionForAnotherUsersSession()
    {
        await TestApp.RunAsUserAsync("owner@local", "Testing1234!", []);
        var session = await StartSessionAsync("Owner's task");

        await TestApp.RunAsDefaultUserAsync();

        await Should.ThrowAsync<NotFoundException>(() =>
            TestApp.SendAsync(new ReportDistractionCommand(
                session.Id,
                DistractionReason.TaskTooDifficult)));

        (await TestApp.CountAsync<DistractionEvent>()).ShouldBe(0);
    }

    [Test]
    public async Task ShouldNotReportDistractionForCompletedSession()
    {
        await TestApp.RunAsDefaultUserAsync();
        var session = await StartSessionAsync("Prepare the release notes");
        await TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id));

        await Should.ThrowAsync<NotFoundException>(() =>
            TestApp.SendAsync(new ReportDistractionCommand(
                session.Id,
                DistractionReason.Tired)));

        (await TestApp.CountAsync<DistractionEvent>()).ShouldBe(0);
    }

    private static async Task<FocusSessionDto> StartSessionAsync(string originalInput)
    {
        var task = await TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = originalInput
        });
        var plan = await TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en"));

        return await TestApp.SendAsync(new StartFocusSessionCommand(task.Id, plan.Id));
    }
}
