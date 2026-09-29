using FocusPocuss.Application.FocusSessions.Queries.GetFocusSessionHistory;
using FocusPocuss.Application.Tasks;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Domain.Entities;
using FocusPocuss.Domain.Enums;

namespace FocusPocuss.Application.FunctionalTests.FocusSessions.Queries;

public class GetFocusSessionHistoryTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(
            () => TestApp.SendAsync(new GetFocusSessionHistoryQuery()));
    }

    [Test]
    public async Task ShouldReturnEmptyHistoryWhenUserHasNoSessions()
    {
        await TestApp.RunAsDefaultUserAsync();

        var result = await TestApp.SendAsync(new GetFocusSessionHistoryQuery());

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task ShouldReturnOnlyCurrentUsersSessions()
    {
        var firstUserId = await TestApp.RunAsUserAsync(
            "first@local",
            "Testing1234!",
            []);
        await CreateSessionAsync(
            firstUserId,
            "First user's session",
            DateTimeOffset.UtcNow.AddHours(-2));

        var secondUserId = await TestApp.RunAsUserAsync(
            "second@local",
            "Testing1234!",
            []);
        var expected = await CreateSessionAsync(
            secondUserId,
            "Second user's session",
            DateTimeOffset.UtcNow.AddHours(-1));

        var result = await TestApp.SendAsync(new GetFocusSessionHistoryQuery());

        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(expected.Id);
        result[0].Action.ShouldBe(expected.Action);
    }

    [Test]
    public async Task ShouldReturnCompletedAndIncompleteSessionsNewestFirst()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();
        var now = DateTimeOffset.UtcNow;
        var oldest = await CreateSessionAsync(
            userId,
            "Oldest session",
            now.AddHours(-3),
            completed: true);
        var newest = await CreateSessionAsync(
            userId,
            "Newest session",
            now.AddHours(-1));
        var middle = await CreateSessionAsync(
            userId,
            "Middle session",
            now.AddHours(-2),
            completed: true);

        var result = await TestApp.SendAsync(new GetFocusSessionHistoryQuery());

        result.Select(session => session.Id).ShouldBe([
            newest.Id,
            middle.Id,
            oldest.Id
        ]);
        result[0].CompletedAtUtc.ShouldBeNull();
        result[1].CompletedAtUtc.ShouldNotBeNull();
        result[2].CompletedAtUtc.ShouldNotBeNull();
    }

    [Test]
    public async Task ShouldIncludeReflectionData()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();
        var startedAtUtc = DateTimeOffset.UtcNow.AddHours(-1);
        var reflectedAtUtc = startedAtUtc.AddMinutes(12);
        var session = await CreateSessionAsync(
            userId,
            "Reflected session",
            startedAtUtc,
            completed: true,
            reflection: FocusSessionReflection.SomeDifficulty,
            reflectedAtUtc: reflectedAtUtc);

        var result = await TestApp.SendAsync(new GetFocusSessionHistoryQuery());

        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(session.Id);
        result[0].Reflection.ShouldBe(FocusSessionReflection.SomeDifficulty);
        var actualReflectedAtUtc = result[0].ReflectedAtUtc;
        actualReflectedAtUtc.ShouldNotBeNull();
        actualReflectedAtUtc.Value.ShouldBe(
            reflectedAtUtc,
            DatabaseTimestampPrecision);
    }

    private static async Task<FocusSession> CreateSessionAsync(
        string userId,
        string action,
        DateTimeOffset startedAtUtc,
        bool completed = false,
        FocusSessionReflection? reflection = null,
        DateTimeOffset? reflectedAtUtc = null)
    {
        var (task, plan) = await CreateTaskAndPlanAsync(action);
        var session = new FocusSession(
            userId,
            task.Id,
            plan.Id,
            action,
            plan.SuggestedDurationMinutes,
            startedAtUtc);

        if (completed)
        {
            session.Complete(startedAtUtc.AddMinutes(plan.SuggestedDurationMinutes));
        }

        if (reflection.HasValue)
        {
            session.RecordReflection(reflection.Value, reflectedAtUtc!.Value);
        }

        await TestApp.AddAsync(session);

        return session;
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
