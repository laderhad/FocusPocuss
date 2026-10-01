using System.Text.Json;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

    [TestCase(DistractionReason.TaskTooDifficult, InterventionStrategy.TaskDecomposition,
        RecoveryInterventionType.ShrinkCurrentAction)]
    [TestCase(DistractionReason.UnclearNextAction, InterventionStrategy.ClarifyNextAction,
        RecoveryInterventionType.ClarifyCurrentAction)]
    [TestCase(DistractionReason.PhoneOrSocialMedia, InterventionStrategy.RemoveFriction,
        RecoveryInterventionType.EnvironmentalReset)]
    [TestCase(DistractionReason.AnotherThought, InterventionStrategy.DistractionRecovery,
        RecoveryInterventionType.ParkThought)]
    [TestCase(DistractionReason.Tired, InterventionStrategy.BreakRecommendation,
        RecoveryInterventionType.RecoveryChoice)]
    [TestCase(DistractionReason.Other, InterventionStrategy.DistractionRecovery,
        RecoveryInterventionType.ReconnectToCurrentAction)]
    public async Task ShouldPersistDistractionForOwnedActiveSession(
        DistractionReason reason, InterventionStrategy legacyStrategy, RecoveryInterventionType type)
    {
        await TestApp.RunAsDefaultUserAsync();
        var session = await StartSessionAsync("Prepare the release notes");

        var result = await TestApp.SendAsync(new ReportDistractionCommand(
            session.Id,
            reason));

        var entity = await TestApp.FindAsync<DistractionEvent>(result.Id);

        entity.ShouldNotBeNull();
        entity.FocusSessionId.ShouldBe(session.Id);
        entity.Reason.ShouldBe(reason);
        entity.OccurredAtUtc.Offset.ShouldBe(TimeSpan.Zero);
        entity.OccurredAtUtc.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));

        result.FocusSessionId.ShouldBe(entity.FocusSessionId);
        result.Reason.ShouldBe(entity.Reason);
        result.OccurredAtUtc.ShouldBe(entity.OccurredAtUtc, DatabaseTimestampPrecision);
        result.Strategy.ShouldBe(legacyStrategy);
        result.Intervention.Type.ShouldBe(type);
        result.Intervention.Version.ShouldBe("distraction-recovery-v1");
        result.Intervention.CurrentAction.ShouldBe(session.Action);
        result.Intervention.ReturnAction.ShouldBe(
            reason is DistractionReason.TaskTooDifficult or DistractionReason.UnclearNextAction
                ? null : session.Action);

        var persistedSession = await TestApp.FindAsync<FocusSession>(session.Id);
        persistedSession.ShouldNotBeNull();
        persistedSession.Action.ShouldBe(session.Action);
        persistedSession.CompletedAtUtc.ShouldBeNull();
        var startPlan = await TestApp.FindAsync<TaskStartPlan>(session.TaskStartPlanId);
        startPlan.ShouldNotBeNull();
        startPlan.NextAction.ShouldBe(session.Action);
        (await TestApp.CountAsync<DistractionEvent>()).ShouldBe(1);

        // Exercise the actual Web JSON configuration, keeping the old field intact
        // while exposing stable string identifiers to future recovery consumers.
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();
        var options = scope.ServiceProvider
            .GetRequiredService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>().Value;
        var json = JsonSerializer.SerializeToElement(result, options.SerializerOptions);
        json.GetProperty("strategy").GetString().ShouldBe(legacyStrategy.ToString());
        var intervention = json.GetProperty("intervention");
        intervention.GetProperty("type").GetString().ShouldBe(type.ToString());
        intervention.GetProperty("requirement").GetString()
            .ShouldBe(result.Intervention.Requirement.ToString());
        intervention.GetProperty("choices").EnumerateArray().Select(choice => choice.GetString())
            .ShouldBe(result.Intervention.Choices.Select(choice => choice.ToString()));
    }

    [Test]
    public async Task ShouldNotReportDistractionForMissingSession()
    {
        await TestApp.RunAsDefaultUserAsync();

        await Should.ThrowAsync<NotFoundException>(() =>
            TestApp.SendAsync(new ReportDistractionCommand(999, DistractionReason.Other)));

        (await TestApp.CountAsync<DistractionEvent>()).ShouldBe(0);
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
