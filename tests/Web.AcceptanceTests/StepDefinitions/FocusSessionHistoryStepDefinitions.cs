namespace FocusPocuss.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class FocusSessionHistoryStepDefinitions(FocusSessionHistoryPage historyPage)
{
    [BeforeFeature("SessionHistory")]
    public static async Task BeforeFeature(IObjectContainer container)
    {
        var browserContext = await PlaywrightSetup.NewContextAsync(new BrowserNewContextOptions
        {
            Locale = "en-US",
        });
        var page = await browserContext.NewPageAsync();

        await page.RouteAsync("**/api/focus-sessions/history", async route =>
        {
            const string responseBody = """
                [
                  {
                    "id": 82,
                    "taskId": 12,
                    "action": "Review the final release checklist.",
                    "plannedDurationMinutes": 12,
                    "startedAtUtc": "2026-09-29T10:00:00Z",
                    "completedAtUtc": null,
                    "reflection": null,
                    "reflectedAtUtc": null
                  },
                  {
                    "id": 81,
                    "taskId": 11,
                    "action": "Draft the release summary.",
                    "plannedDurationMinutes": 10,
                    "startedAtUtc": "2026-09-29T09:00:00Z",
                    "completedAtUtc": "2026-09-29T09:10:00Z",
                    "reflection": "FocusedWell",
                    "reflectedAtUtc": "2026-09-29T09:11:00Z"
                  }
                ]
                """;

            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = responseBody,
            });
        });

        var loginPage = new LoginPage(page);
        await loginPage.GotoAsync();
        await loginPage.SetEmail("administrator@localhost");
        await loginPage.SetPassword("Administrator1!");
        await loginPage.ClickLogin();
        await Assertions.Expect(page.Locator("#email")).ToHaveCountAsync(0);

        container.RegisterInstanceAs(browserContext);
        container.RegisterInstanceAs(new FocusSessionHistoryPage(page));
    }

    [AfterFeature("SessionHistory")]
    public static async Task AfterFeature(IObjectContainer container)
    {
        await container.Resolve<IBrowserContext>().DisposeAsync();
    }

    [Given("an authenticated user has focus session history")]
    public static Task GivenAnAuthenticatedUserHasFocusSessionHistory()
    {
        return Task.CompletedTask;
    }

    [When("the user opens focus session history")]
    public async Task WhenTheUserOpensFocusSessionHistory()
    {
        await historyPage.GotoAsync();
    }

    [Then("completed and active sessions are shown newest first")]
    public async Task ThenCompletedAndActiveSessionsAreShownNewestFirst()
    {
        await historyPage.AssertSessionsNewestFirstAsync();
    }

    [Then("the saved reflection is shown")]
    public async Task ThenTheSavedReflectionIsShown()
    {
        await historyPage.AssertReflectionAsync();
    }
}
