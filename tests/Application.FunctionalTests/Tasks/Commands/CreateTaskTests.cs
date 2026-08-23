using FocusPocuss.Application.Common.Exceptions;
using FocusPocuss.Application.Tasks.Commands.CreateTask;
using FocusPocuss.Domain.Entities;

namespace FocusPocuss.Application.FunctionalTests.Tasks.Commands;

public class CreateTaskTests : TestBase
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("\t\r\n")]
    public async Task ShouldRequireMeaningfulOriginalInput(string? originalInput)
    {
        await TestApp.RunAsDefaultUserAsync();

        var command = new CreateTaskCommand
        {
            OriginalInput = originalInput!
        };

        await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldRejectOriginalInputOverMaximumLength()
    {
        await TestApp.RunAsDefaultUserAsync();

        var command = new CreateTaskCommand
        {
            OriginalInput = new string('a', 1001)
        };

        await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var command = new CreateTaskCommand
        {
            OriginalInput = "Prepare the report"
        };

        await Should.ThrowAsync<UnauthorizedAccessException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldPersistExactOriginalInputForCurrentUser()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();
        const string originalInput = "  I need to study algorithms,\nbut I do not know where to start.  ";

        var result = await TestApp.SendAsync(new CreateTaskCommand
        {
            OriginalInput = originalInput
        });

        var entity = await TestApp.FindAsync<TaskItem>(result.Id);

        entity.ShouldNotBeNull();
        entity.UserId.ShouldBe(userId);
        entity.OriginalInput.ShouldBe(originalInput);
        entity.CreatedBy.ShouldBe(userId);
        entity.Created.ShouldBe(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        entity.LastModifiedBy.ShouldBe(userId);
        entity.LastModified.ShouldBe(DateTimeOffset.Now, TimeSpan.FromSeconds(10));

        result.OriginalInput.ShouldBe(originalInput);
        result.CreatedAt.ShouldBe(entity.Created);
    }
}
