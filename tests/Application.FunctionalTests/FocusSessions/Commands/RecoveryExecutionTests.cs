using FocusPocuss.Application.Behavior.Interventions;
using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.FocusSessions;
using FocusPocuss.Application.FocusSessions.Commands.CompleteFocusSession;
using FocusPocuss.Application.FocusSessions.Commands.PrepareRecovery;
using FocusPocuss.Application.FocusSessions.Commands.ReportDistraction;
using FocusPocuss.Application.FocusSessions.Commands.ResolveRecovery;
using FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;
using FocusPocuss.Application.FocusSessions.Queries.GetFocusSession;
using FocusPocuss.Application.FocusSessions.Queries.GetFocusSessionHistory;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;
using FocusPocuss.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FocusPocuss.Application.FunctionalTests.FocusSessions.Commands;

public class RecoveryExecutionTests : TestBase
{
    [TestCase("tr", "Soruda verilenleri yaz.")]
    [TestCase("en", "Write the information given in the question.")]
    public async Task ShouldApplySmallerActionDurablyWithoutChangingOriginalPlan(string language, string action)
    {
        var session = await StartAsync();
        TestApp.GetRecoveryActionPlanner().Result = new(RecoveryActionStatus.Ready, action);
        var report = await ReportAsync(session, DistractionReason.TaskTooDifficult, language);
        report.Intervention.ReturnAction.ShouldBe(action);
        (await GetAsync(session)).Action.ShouldBe(session.Action);
        (await GetAsync(session)).PendingRecovery!.Id.ShouldBe(report.Id);

        await ResolveAsync(session, report, RecoveryResolution.ReturnToFocus);
        var reloaded = await GetAsync(session);
        reloaded.Action.ShouldBe(action);
        reloaded.OriginalAction.ShouldBe(session.Action);
        reloaded.PendingRecovery.ShouldBeNull();
        var plan = await TestApp.FindAsync<TaskStartPlan>(session.TaskStartPlanId);
        plan!.NextAction.ShouldBe(session.Action);
        TestApp.GetRecoveryActionPlanner().Requests.Single().Language.ShouldBe(language);
        var saved = await TestApp.FindAsync<DistractionEvent>(report.Id);
        saved!.StrategyVersion.ShouldBe("distraction-recovery-v1");
        saved.Resolution.ShouldBe(RecoveryResolution.ReturnToFocus);
        saved.ResolvedAtUtc.ShouldNotBeNull();
    }

    [Test]
    public async Task ShouldParkThoughtAndReturnToSameActionWithoutAI()
    {
        var session = await StartAsync();
        var report = await ReportAsync(session, DistractionReason.AnotherThought);
        await Should.ThrowAsync<ValidationException>(() => ResolveAsync(session, report, RecoveryResolution.ReturnToFocus));
        await ResolveAsync(session, report, RecoveryResolution.ReturnToFocus, "Remember to call tomorrow.");
        var reloaded = await GetAsync(session);
        reloaded.Action.ShouldBe(session.Action);
        reloaded.ParkedThoughts.Single().Text.ShouldBe("Remember to call tomorrow.");
        (await TestApp.SendAsync(new GetFocusSessionHistoryQuery())).Single().ParkedThoughtCount.ShouldBe(1);
        TestApp.GetRecoveryActionPlanner().Requests.ShouldBeEmpty();
        await TestApp.RunAsUserAsync("another@local", "Testing1234!", []);
        await Should.ThrowAsync<NotFoundException>(() => GetAsync(session));
    }

    [TestCase(DistractionReason.PhoneOrSocialMedia)]
    [TestCase(DistractionReason.Other)]
    public async Task ShouldRestoreSameActionWithoutAI(DistractionReason reason)
    {
        var session = await StartAsync();
        var report = await ReportAsync(session, reason);
        (await ResolveAsync(session, report, RecoveryResolution.ReturnToFocus)).Action.ShouldBe(session.Action);
        TestApp.GetRecoveryActionPlanner().Requests.ShouldBeEmpty();
    }

    [Test]
    public async Task ShouldAskForOnlyOneClarificationAndAllowSafeFallback()
    {
        var session = await StartAsync();
        var report = await ReportAsync(session, DistractionReason.UnclearNextAction);
        report.Intervention.Requirement.ShouldBe(RecoveryRequirement.NeedsClarification);
        await Should.ThrowAsync<ConflictException>(() => ResolveAsync(session, report, RecoveryResolution.ReturnToFocus));
        var clarified = await TestApp.SendAsync(new PrepareRecoveryCommand(session.Id, report.Id,
            Clarification: "The report's current blank field is unclear."));
        clarified.ClarificationUsed.ShouldBeTrue();
        clarified.Intervention.Requirement.ShouldBe(RecoveryRequirement.Unavailable);
        await Should.ThrowAsync<ConflictException>(() => TestApp.SendAsync(new PrepareRecoveryCommand(
            session.Id, report.Id, Clarification: "Another message")));
        (await ResolveAsync(session, report, RecoveryResolution.KeepCurrentAction)).Action.ShouldBe(session.Action);
        TestApp.GetRecoveryActionPlanner().Requests.Count.ShouldBe(2);
    }

    [Test]
    public async Task ShouldApplyGroundedClarification()
    {
        var session = await StartAsync();
        var report = await ReportAsync(session, DistractionReason.UnclearNextAction);
        TestApp.GetRecoveryActionPlanner().Result = new(RecoveryActionStatus.Ready, "Write the observed login result.");
        var clarified = await TestApp.SendAsync(new PrepareRecoveryCommand(session.Id, report.Id,
            Clarification: "The login succeeds but the page stays unchanged."));
        clarified.Intervention.Requirement.ShouldBe(RecoveryRequirement.None);
        (await ResolveAsync(session, report, RecoveryResolution.ReturnToFocus)).Action
            .ShouldBe("Write the observed login result.");
    }

    [TestCase(RecoveryChoice.TakeShortReset)]
    [TestCase(RecoveryChoice.ContinueWithSmallerAction)]
    public async Task ShouldExecuteTirednessChoice(RecoveryChoice choice)
    {
        var session = await StartAsync();
        TestApp.GetRecoveryActionPlanner().Result = new(RecoveryActionStatus.Ready, "Write one given value.");
        var report = await ReportAsync(session, DistractionReason.Tired);
        await Should.ThrowAsync<ConflictException>(() => ResolveAsync(session, report, RecoveryResolution.ReturnToFocus));
        var prepared = await TestApp.SendAsync(new PrepareRecoveryCommand(session.Id, report.Id, choice));
        prepared.Choice.ShouldBe(choice);
        var result = await ResolveAsync(session, report, RecoveryResolution.ReturnToFocus);
        result.Action.ShouldBe(choice == RecoveryChoice.TakeShortReset ? session.Action : "Write one given value.");
        TestApp.GetRecoveryActionPlanner().Requests.Count.ShouldBe(choice == RecoveryChoice.TakeShortReset ? 0 : 1);
    }

    [Test]
    public async Task ShouldEndEarlyWithoutRecordingCompletionAndAllowANewSession()
    {
        var session = await StartAsync();
        var report = await ReportAsync(session, DistractionReason.Tired);
        var result = await ResolveAsync(session, report, RecoveryResolution.EndSession);
        result.EndedEarlyAtUtc.ShouldNotBeNull();
        result.CompletedAtUtc.ShouldBeNull();
        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id)));
        var history = (await TestApp.SendAsync(new GetFocusSessionHistoryQuery())).Single();
        history.CompletedAtUtc.ShouldBeNull();
        history.EndedEarlyAtUtc.ShouldNotBeNull();
        var next = await TestApp.SendAsync(new StartFocusSessionCommand(session.TaskId, session.TaskStartPlanId));
        next.Id.ShouldNotBe(session.Id);
    }

    [Test]
    public async Task ShouldResumePendingRecoveryAndNeverReapplyResolvedActions()
    {
        var session = await StartAsync();
        var first = await ReportAsync(session, DistractionReason.PhoneOrSocialMedia);
        var duplicate = await ReportAsync(session, DistractionReason.Other);
        duplicate.Id.ShouldBe(first.Id);
        (await TestApp.CountAsync<DistractionEvent>()).ShouldBe(1);
        await ResolveAsync(session, first, RecoveryResolution.ReturnToFocus);
        TestApp.GetRecoveryActionPlanner().Result = new(RecoveryActionStatus.Ready, "Write one given value.");
        var second = await ReportAsync(session, DistractionReason.TaskTooDifficult);
        await ResolveAsync(session, second, RecoveryResolution.ReturnToFocus);
        (await ResolveAsync(session, first, RecoveryResolution.ReturnToFocus)).Action.ShouldBe("Write one given value.");
        await Should.ThrowAsync<ConflictException>(() => ResolveAsync(session, first, RecoveryResolution.Dismiss));
    }

    [Test]
    public async Task ShouldDismissWithoutApplyingOrRecordingAReturn()
    {
        var session = await StartAsync();
        TestApp.GetRecoveryActionPlanner().Result = new(RecoveryActionStatus.Ready, "Write one value.");
        var report = await ReportAsync(session, DistractionReason.TaskTooDifficult);
        var result = await ResolveAsync(session, report, RecoveryResolution.Dismiss);
        result.Action.ShouldBe(session.Action);
        (await TestApp.FindAsync<DistractionEvent>(report.Id))!.Resolution.ShouldBe(RecoveryResolution.Dismiss);
    }

    [Test]
    public async Task ShouldProtectRecoveryOwnershipAndEndedSessions()
    {
        var session = await StartAsync();
        var report = await ReportAsync(session, DistractionReason.UnclearNextAction);
        await TestApp.RunAsUserAsync("intruder@local", "Testing1234!", []);
        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(
            new PrepareRecoveryCommand(session.Id, report.Id, Clarification: "private")));
        await Should.ThrowAsync<NotFoundException>(() => ResolveAsync(session, report, RecoveryResolution.KeepCurrentAction));
    }

    [Test]
    public async Task ShouldDenyAnonymousRecoveryCommands()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(() => TestApp.SendAsync(
            new PrepareRecoveryCommand(1, 1, Clarification: "context")));
        await Should.ThrowAsync<UnauthorizedAccessException>(() => TestApp.SendAsync(
            new ResolveRecoveryCommand(1, 1, RecoveryResolution.Dismiss)));
    }

    [Test]
    public async Task ShouldRejectRecoveryAfterCompletion()
    {
        var session = await StartAsync();
        var report = await ReportAsync(session, DistractionReason.UnclearNextAction);
        await TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id));
        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(
            new PrepareRecoveryCommand(session.Id, report.Id, Clarification: "context")));
        await Should.ThrowAsync<ConflictException>(() => ResolveAsync(session, report, RecoveryResolution.KeepCurrentAction));
    }

    [Test]
    public async Task ShouldRejectConcurrentSessionChanges()
    {
        var session = await StartAsync();
        using var firstScope = FunctionalTestSetup.ScopeFactory.CreateScope();
        using var secondScope = FunctionalTestSetup.ScopeFactory.CreateScope();
        var first = firstScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var second = secondScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var firstSession = (await first.FocusSessions.FindAsync(session.Id))!;
        var secondSession = (await second.FocusSessions.FindAsync(session.Id))!;
        firstSession.TouchRecovery();
        secondSession.Complete(DateTimeOffset.UtcNow);
        await first.SaveChangesAsync();
        await Should.ThrowAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());
    }

    [Test]
    public async Task ShouldResumeOneRecoveryWhenTwoTabsReportTogether()
    {
        var session = await StartAsync();
        var reports = await Task.WhenAll(
            ReportAsync(session, DistractionReason.PhoneOrSocialMedia),
            ReportAsync(session, DistractionReason.Other));
        reports[0].Id.ShouldBe(reports[1].Id);
        (await TestApp.CountAsync<DistractionEvent>()).ShouldBe(1);
    }

    private static async Task<FocusSessionDto> StartAsync()
    {
        await TestApp.RunAsDefaultUserAsync();
        var task = await TestApp.SendAsync(new CreateTaskCommand { OriginalInput = "Study" });
        var plan = await TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en"));
        return await TestApp.SendAsync(new StartFocusSessionCommand(task.Id, plan.Id));
    }
    private static Task<DistractionReportDto> ReportAsync(FocusSessionDto session, DistractionReason reason, string language = "en")
        => TestApp.SendAsync(new ReportDistractionCommand(session.Id, reason, language));
    private static Task<FocusSessionDto> GetAsync(FocusSessionDto session)
        => TestApp.SendAsync(new GetFocusSessionQuery(session.Id));
    private static Task<FocusSessionDto> ResolveAsync(FocusSessionDto session, DistractionReportDto report,
        RecoveryResolution resolution, string? thought = null)
        => TestApp.SendAsync(new ResolveRecoveryCommand(session.Id, report.Id, resolution, thought));
}
