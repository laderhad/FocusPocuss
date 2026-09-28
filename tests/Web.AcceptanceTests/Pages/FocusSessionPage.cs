using System.Text.RegularExpressions;

namespace FocusPocuss.Web.AcceptanceTests.Pages;

public class FocusSessionPage(IPage page) : BasePage(page)
{
    public const int SessionId = 73;
    public const string ExpectedNextAction = "Open the project and write the first review note.";

    public override string PagePath => $"{BaseUrl}/focus/{SessionId}";

    public async Task StartAsync()
    {
        await Page.Locator(".focus-start-button").ClickAsync();
        await Assertions.Expect(Page).ToHaveURLAsync(new Regex($@"/focus/{SessionId}$"));
    }

    public async Task AssertFocusedActionWithoutNavigationAsync()
    {
        await Assertions.Expect(Page.Locator(".focus-session h1"))
            .ToHaveTextAsync(ExpectedNextAction);
        await Assertions.Expect(Page.Locator("nav")).ToHaveCountAsync(0);
    }

    public async Task AssertExpiredSessionRemainsActiveAsync()
    {
        await Assertions.Expect(Page.Locator(".focus-session-timer time"))
            .ToHaveTextAsync("00:00");
        await Assertions.Expect(Page.Locator(".focus-session-expired"))
            .ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator(".focus-session button:has-text('Complete session')"))
            .ToHaveTextAsync("Complete session");
    }

    public Task CompleteAsync()
        => Page.Locator(".focus-session button:has-text('Complete session')").ClickAsync();

    public Task AssertCompletedAsync()
        => Assertions.Expect(Page.Locator("#focus-completed-title"))
            .ToHaveTextAsync("Session completed");

    public async Task ReportUnclearNextActionDistractionAsync()
    {
        await Page.Locator(".focus-distraction-trigger").ClickAsync();
        await Page.GetByLabel("The next action is unclear").CheckAsync();
        await Page.Locator(".focus-distraction-form button[type='submit']").ClickAsync();
    }

    public async Task AssertRecoveryGuidanceAsync()
    {
        await Assertions.Expect(Page.Locator(".focus-distraction-success"))
            .ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator(".focus-distraction-success-title"))
            .ToHaveTextAsync("Try this now");
        await Assertions.Expect(Page.Locator(".focus-distraction-message"))
            .ToHaveTextAsync("Name the next physical action, then do only that.");
        await Assertions.Expect(Page.Locator(".focus-distraction-return"))
            .ToHaveTextAsync("Return to focus");
    }

    public Task ReturnToFocusAsync()
        => Page.Locator(".focus-distraction-return").ClickAsync();

    public async Task AssertRecoveryDismissedAndSessionActiveAsync()
    {
        await Assertions.Expect(Page.Locator(".focus-distraction-success"))
            .ToHaveCountAsync(0);
        await Assertions.Expect(Page.Locator(".focus-distraction-trigger"))
            .ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator(".focus-session h1"))
            .ToHaveTextAsync(ExpectedNextAction);
        await Assertions.Expect(Page.Locator(".focus-session button:has-text('Complete session')"))
            .ToBeVisibleAsync();
    }
}
