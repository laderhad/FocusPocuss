using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.FocusSessions;
using FocusPocuss.Application.FocusSessions.Commands.CompleteFocusSession;
using FocusPocuss.Application.FocusSessions.Commands.RecordFocusSessionReflection;
using FocusPocuss.Application.FocusSessions.Commands.StartFocusSession;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FunctionalTests.FocusSessions.Commands;

public class RecordFocusSessionReflectionTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            TestApp.SendAsync(new RecordFocusSessionReflectionCommand(
                1,
                FocusSessionReflection.FocusedWell)));
    }

    [Test]
    public async Task ShouldRequireSupportedReflection()
    {
        await TestApp.RunAsDefaultUserAsync();

        await Should.ThrowAsync<ValidationException>(() =>
            TestApp.SendAsync(new RecordFocusSessionReflectionCommand(
                1,
                (FocusSessionReflection)999)));
    }

    [Test]
    public async Task ShouldPersistReflectionForCompletedOwnedSession()
    {
        await TestApp.RunAsDefaultUserAsync();
        var session = await StartSessionAsync("Prepare the release notes");
        await TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id));

        var result = await TestApp.SendAsync(new RecordFocusSessionReflectionCommand(
            session.Id,
            FocusSessionReflection.SomeDifficulty));

        var entity = await TestApp.FindAsync<FocusSession>(session.Id);

        entity.ShouldNotBeNull();
        entity.Reflection.ShouldBe(FocusSessionReflection.SomeDifficulty);
        entity.ReflectedAtUtc.ShouldNotBeNull();
        entity.ReflectedAtUtc.Value.Offset.ShouldBe(TimeSpan.Zero);

        result.Reflection.ShouldBe(entity.Reflection);
        result.ReflectedAtUtc.ShouldNotBeNull();
        result.ReflectedAtUtc.Value.ShouldBe(
            entity.ReflectedAtUtc.Value,
            DatabaseTimestampPrecision);
    }

    [Test]
    public async Task ShouldNotRecordReflectionForActiveSession()
    {
        await TestApp.RunAsDefaultUserAsync();
        var session = await StartSessionAsync("Prepare the release notes");

        await Should.ThrowAsync<NotFoundException>(() =>
            TestApp.SendAsync(new RecordFocusSessionReflectionCommand(
                session.Id,
                FocusSessionReflection.FocusedWell)));

        (await TestApp.FindAsync<FocusSession>(session.Id))!.Reflection.ShouldBeNull();
    }

    [Test]
    public async Task ShouldNotRecordReflectionForAnotherUsersSession()
    {
        await TestApp.RunAsUserAsync("owner@local", "Testing1234!", []);
        var session = await StartSessionAsync("Owner's task");
        await TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id));

        await TestApp.RunAsDefaultUserAsync();

        await Should.ThrowAsync<NotFoundException>(() =>
            TestApp.SendAsync(new RecordFocusSessionReflectionCommand(
                session.Id,
                FocusSessionReflection.SignificantDifficulty)));

        (await TestApp.FindAsync<FocusSession>(session.Id))!.Reflection.ShouldBeNull();
    }

    [Test]
    public async Task ShouldUpdateExistingReflection()
    {
        await TestApp.RunAsDefaultUserAsync();
        var session = await StartSessionAsync("Prepare the release notes");
        await TestApp.SendAsync(new CompleteFocusSessionCommand(session.Id));

        var first = await TestApp.SendAsync(new RecordFocusSessionReflectionCommand(
            session.Id,
            FocusSessionReflection.FocusedWell));
        var updated = await TestApp.SendAsync(new RecordFocusSessionReflectionCommand(
            session.Id,
            FocusSessionReflection.SignificantDifficulty));

        var entity = await TestApp.FindAsync<FocusSession>(session.Id);

        entity.ShouldNotBeNull();
        entity.Reflection.ShouldBe(FocusSessionReflection.SignificantDifficulty);
        entity.ReflectedAtUtc.ShouldNotBeNull();
        updated.Reflection.ShouldBe(FocusSessionReflection.SignificantDifficulty);
        updated.ReflectedAtUtc.ShouldNotBeNull();
        updated.ReflectedAtUtc.Value.ShouldBeGreaterThanOrEqualTo(first.ReflectedAtUtc!.Value);
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
