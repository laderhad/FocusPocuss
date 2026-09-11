namespace FocusPocuss.Web.AcceptanceTests.Pages;

public class TaskCapturePage(IPage page) : BasePage(page)
{
    public override string PagePath => $"{BaseUrl}/tasks";

    public async Task CaptureTaskAsync(string originalInput)
    {
        var input = Page.Locator("#task-original-input");

        await input.FillAsync(originalInput);
        await Page.Locator(".task-capture-form button[type='submit']").ClickAsync();
        await Assertions.Expect(input).ToHaveValueAsync(string.Empty);
    }

    public async Task AssertLatestTaskAsync(string originalInput)
    {
        var latestTask = Page.Locator(".task-history-item").First;

        await Assertions.Expect(latestTask).ToContainTextAsync(originalInput.Trim());
        (await latestTask.TextContentAsync()).ShouldBe(originalInput);
    }
}
