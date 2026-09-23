using System.Text.RegularExpressions;

namespace FocusPocuss.Web.AcceptanceTests.Pages;

public class TaskCapturePage(IPage page) : BasePage(page)
{
    public const string ExpectedNextAction = "Open the project and write the first review note.";

    public override string PagePath => $"{BaseUrl}/tasks";

    public async Task CaptureTaskAsync(string originalInput)
    {
        var input = Page.Locator("#task-original-input");

        await input.FillAsync(originalInput);
        await Page.Locator(".task-capture-form button[type='submit']").ClickAsync();
        await Assertions.Expect(Page).ToHaveURLAsync(new Regex(@"/tasks/\d+$"));
    }

    public async Task AssertTaskDetailsAsync(string originalInput)
    {
        var capturedTask = Page.Locator(".task-detail-header h1");

        await Assertions.Expect(capturedTask).ToBeVisibleAsync();
        (await capturedTask.TextContentAsync()).ShouldBe(originalInput);
    }

    public Task AssertStartPlanAsync()
        => Assertions.Expect(Page.Locator(".task-start-plan-action"))
            .ToHaveTextAsync(ExpectedNextAction);
}
