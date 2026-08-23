using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Queries.GetTasks;

namespace FocusPocuss.Application.FunctionalTests.Tasks.Queries;

public class GetTasksTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(
            () => TestApp.SendAsync(new GetTasksQuery()));
    }

    [Test]
    public async Task ShouldReturnOnlyCurrentUsersTasks()
    {
        await TestApp.RunAsUserAsync("first@local", "Testing1234!", []);
        await TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = "First user's task"
        });

        await TestApp.RunAsUserAsync("second@local", "Testing1234!", []);
        var expected = await TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = "Second user's task"
        });

        var result = await TestApp.SendAsync(new GetTasksQuery());

        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(expected.Id);
        result[0].OriginalInput.ShouldBe(expected.OriginalInput);
    }

    [Test]
    public async Task ShouldReturnNewestTasksFirst()
    {
        await TestApp.RunAsDefaultUserAsync();

        var first = await TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = "First task"
        });
        var second = await TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = "Second task"
        });

        var result = await TestApp.SendAsync(new GetTasksQuery());

        result.Select(task => task.Id).ShouldBe([second.Id, first.Id]);
    }
}
