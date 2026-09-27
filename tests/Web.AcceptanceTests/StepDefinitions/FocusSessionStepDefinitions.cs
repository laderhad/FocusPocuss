namespace FocusPocuss.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class FocusSessionStepDefinitions(
    TaskCapturePage taskCapturePage,
    FocusSessionPage focusSessionPage)
{
    [BeforeFeature("FocusSession")]
    public static async Task BeforeFocusSessionFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.NewContextAsync(new BrowserNewContextOptions
        {
            Locale = "en-US"
        });
        var page = await context.NewPageAsync();

        await page.RouteAsync("**/api/tasks/*/start-plan", route =>
            route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = $$"""
                    {
                      "id": 501,
                      "message": "A small first step is enough.",
                      "nextAction": "{{FocusSessionPage.ExpectedNextAction}}",
                      "suggestedDurationMinutes": 10,
                      "createdAt": "2026-09-25T08:00:00Z"
                    }
                    """
            }));
        await page.RouteAsync("**/api/focus-sessions/active", route =>
            FulfillSessionAsync(route, completed: false));
        await page.RouteAsync($"**/api/focus-sessions/{FocusSessionPage.SessionId}", route =>
            FulfillSessionAsync(route, completed: false));
        await page.RouteAsync($"**/api/focus-sessions/{FocusSessionPage.SessionId}/complete", route =>
            FulfillSessionAsync(route, completed: true));
        await page.RouteAsync($"**/api/focus-sessions/{FocusSessionPage.SessionId}/distractions", route =>
            route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = $$"""
                    {
                      "id": 901,
                      "focusSessionId": {{FocusSessionPage.SessionId}},
                      "reason": "UnclearNextAction",
                      "occurredAtUtc": "2026-09-25T08:10:00Z"
                    }
                    """
            }));

        var loginPage = new LoginPage(page);
        await loginPage.GotoAsync();
        await loginPage.SetEmail("administrator@localhost");
        await loginPage.SetPassword("Administrator1!");
        await loginPage.ClickLogin();
        await Assertions.Expect(page.Locator("#email")).ToHaveCountAsync(0);

        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new TaskCapturePage(page));
        container.RegisterInstanceAs(new FocusSessionPage(page));
    }

    [AfterFeature("FocusSession")]
    public static async Task AfterFocusSessionFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("an authenticated user has a task start recommendation")]
    public async Task GivenAnAuthenticatedUserHasATaskStartRecommendation()
    {
        await taskCapturePage.GotoAsync();
        await taskCapturePage.CaptureTaskAsync($"Prepare release notes {Guid.NewGuid():N}");
        await taskCapturePage.AssertStartPlanAsync();
    }

    [When("the user starts the focus session")]
    public Task WhenTheUserStartsTheFocusSession()
        => focusSessionPage.StartAsync();

    [Then("the focused next action is shown without the main navigation")]
    public Task ThenTheFocusedNextActionIsShownWithoutTheMainNavigation()
        => focusSessionPage.AssertFocusedActionWithoutNavigationAsync();

    [Then("the expired focus session remains available to complete")]
    public Task ThenTheExpiredFocusSessionRemainsAvailableToComplete()
        => focusSessionPage.AssertExpiredSessionRemainsActiveAsync();

    [When("the user completes the focus session")]
    public Task WhenTheUserCompletesTheFocusSession()
        => focusSessionPage.CompleteAsync();

    [Then("the focus session is shown as completed")]
    public Task ThenTheFocusSessionIsShownAsCompleted()
        => focusSessionPage.AssertCompletedAsync();

    [When("the user reports an unclear next action distraction")]
    public Task WhenTheUserReportsAnUnclearNextActionDistraction()
        => focusSessionPage.ReportUnclearNextActionDistractionAsync();

    [Then("the distraction is acknowledged and the focus session remains active")]
    public Task ThenTheDistractionIsAcknowledgedAndTheFocusSessionRemainsActive()
        => focusSessionPage.AssertDistractionAcknowledgedAndSessionActiveAsync();

    private static Task FulfillSessionAsync(IRoute route, bool completed)
    {
        var completedAtUtc = completed ? "\"2026-09-25T08:20:00Z\"" : "null";

        return route.FulfillAsync(new RouteFulfillOptions
        {
            Status = 200,
            ContentType = "application/json",
            Body = $$"""
                {
                  "id": {{FocusSessionPage.SessionId}},
                  "taskId": 1,
                  "taskStartPlanId": 501,
                  "action": "{{FocusSessionPage.ExpectedNextAction}}",
                  "plannedDurationMinutes": 10,
                  "startedAtUtc": "2020-01-01T08:00:00Z",
                  "completedAtUtc": {{completedAtUtc}}
                }
                """
        });
    }
}
