using Microsoft.Playwright;

namespace FocusPocuss.Web.AcceptanceTests.Pages;

public sealed class FocusSessionHistoryPage(IPage page) : BasePage(page)
{
    public const string NewestAction = "Review the final release checklist.";
    public const string OlderAction = "Draft the release summary.";

    public override string PagePath => $"{BaseUrl}/history";

    public async Task AssertSessionsNewestFirstAsync()
    {
        await Assertions.Expect(Page.Locator("#session-history-title"))
            .ToHaveTextAsync("How do I work more easily?");

        var sessions = Page.Locator(".session-history-item");
        await Assertions.Expect(sessions).ToHaveCountAsync(2);

        await Assertions.Expect(sessions.Nth(0).Locator("h3"))
            .ToHaveTextAsync(NewestAction);
        await Assertions.Expect(sessions.Nth(0).Locator(".session-history-status"))
            .ToHaveTextAsync("In progress");

        await Assertions.Expect(sessions.Nth(1).Locator("h3"))
            .ToHaveTextAsync(OlderAction);
        await Assertions.Expect(sessions.Nth(1).Locator(".session-history-status"))
            .ToHaveTextAsync("Completed");
    }

    public async Task AssertReflectionAsync()
    {
        var olderSession = Page.Locator(".session-history-item").Nth(1);

        await Assertions.Expect(olderSession.Locator(".session-history-reflection"))
            .ToHaveTextAsync("Reflection: I focused well");
    }
}
