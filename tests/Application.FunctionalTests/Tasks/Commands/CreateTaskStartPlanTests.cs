using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Application.Tasks.Commands.CreateTaskStartPlan;
using FocusPocuss.Application.Tasks.Planning;
using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.FunctionalTests.Tasks.Commands;

public class CreateTaskStartPlanTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(
            () => TestApp.SendAsync(new CreateTaskStartPlanCommand(1, "en")));
    }

    [TestCase("")]
    [TestCase("de")]
    [TestCase("TR")]
    public async Task ShouldRequireSupportedLanguage(string language)
    {
        await TestApp.RunAsDefaultUserAsync();

        await Should.ThrowAsync<ValidationException>(
            () => TestApp.SendAsync(new CreateTaskStartPlanCommand(1, language)));
    }

    [Test]
    public async Task ShouldNotCreatePlanForAnotherUsersTask()
    {
        await TestApp.RunAsUserAsync("first@local", "Testing1234!", []);
        var task = await CreateTaskAsync("First user's task");

        await TestApp.RunAsUserAsync("second@local", "Testing1234!", []);

        await Should.ThrowAsync<NotFoundException>(
            () => TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en")));

        TestApp.GetTaskStartPlanner().Requests.ShouldBeEmpty();
        (await TestApp.CountAsync<TaskStartPlan>()).ShouldBe(0);
    }

    [Test]
    public async Task ShouldPersistPlanForExactOriginalInput()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();
        const string originalInput = "  Prepare the API review,\nbut I do not know where to begin.  ";
        var task = await CreateTaskAsync(originalInput);
        var planner = TestApp.GetTaskStartPlanner();
        planner.Result = new TaskStartPlanResult(
            "Let's make the first move small.",
            "Open the API project and list the first missing endpoint.",
            12,
            "configured-test-model",
            "task-start-plan-test-v1");

        var result = await TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en"));

        planner.Requests.Count.ShouldBe(1);
        planner.Requests[0].OriginalInput.ShouldBe(originalInput);
        planner.Requests[0].Language.ShouldBe("en");

        var entity = await TestApp.FindAsync<TaskStartPlan>(result.Id);
        entity.ShouldNotBeNull();
        entity.TaskItemId.ShouldBe(task.Id);
        entity.Message.ShouldBe(planner.Result.Message);
        entity.NextAction.ShouldBe(planner.Result.NextAction);
        entity.SuggestedDurationMinutes.ShouldBe(12);
        entity.Language.ShouldBe("en");
        entity.Model.ShouldBe("configured-test-model");
        entity.PromptVersion.ShouldBe("task-start-plan-test-v1");
        entity.CreatedBy.ShouldBe(userId);

        result.Message.ShouldBe(entity.Message);
        result.NextAction.ShouldBe(entity.NextAction);
        result.SuggestedDurationMinutes.ShouldBe(entity.SuggestedDurationMinutes);
    }

    [Test]
    public async Task ShouldReturnExistingPlanWithoutCallingPlannerAgain()
    {
        await TestApp.RunAsDefaultUserAsync();
        var task = await CreateTaskAsync("Prepare the release notes");
        var planner = TestApp.GetTaskStartPlanner();

        var first = await TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en"));
        planner.Result = planner.Result with { Message = "This result must not be used." };

        var second = await TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "tr"));

        second.Id.ShouldBe(first.Id);
        second.Message.ShouldBe(first.Message);
        planner.Requests.Count.ShouldBe(1);
        (await TestApp.CountAsync<TaskStartPlan>()).ShouldBe(1);
    }

    [Test]
    public async Task ShouldNotPersistInvalidPlannerResult()
    {
        await TestApp.RunAsDefaultUserAsync();
        var task = await CreateTaskAsync("Prepare the release notes");
        var planner = TestApp.GetTaskStartPlanner();
        planner.Result = planner.Result with { SuggestedDurationMinutes = 0 };

        await Should.ThrowAsync<InvalidOperationException>(
            () => TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en")));

        (await TestApp.CountAsync<TaskStartPlan>()).ShouldBe(0);
    }

    [Test]
    public async Task ShouldNotPersistPlanWhenPlannerFails()
    {
        await TestApp.RunAsDefaultUserAsync();
        var task = await CreateTaskAsync("Prepare the release notes");
        var planner = TestApp.GetTaskStartPlanner();
        planner.ExceptionToThrow = new InvalidOperationException("Planner unavailable.");

        await Should.ThrowAsync<InvalidOperationException>(
            () => TestApp.SendAsync(new CreateTaskStartPlanCommand(task.Id, "en")));

        (await TestApp.CountAsync<TaskStartPlan>()).ShouldBe(0);
    }

    private static Task<FocusPocuss.Application.Tasks.TaskDto> CreateTaskAsync(string originalInput)
    {
        return TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = originalInput
        });
    }
}
