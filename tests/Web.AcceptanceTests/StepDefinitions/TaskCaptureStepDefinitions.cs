namespace FocusPocuss.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class TaskCaptureStepDefinitions(TaskCapturePage taskCapturePage)
{
    private string _originalInput = null!;

    [BeforeFeature("TaskCapture")]
    public static async Task BeforeTaskCaptureFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.NewContextAsync(new BrowserNewContextOptions
        {
            Locale = "en-US"
        });
        var page = await context.NewPageAsync();

        await page.RouteAsync("**/api/tasks/*/start-plan", async route =>
        {
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = $$"""
                    {
                      "id": 1,
                      "message": "A small first step is enough.",
                      "nextAction": "{{TaskCapturePage.ExpectedNextAction}}",
                      "suggestedDurationMinutes": 8,
                      "createdAt": "2026-09-18T10:00:00Z"
                    }
                    """
            });
        });

        var loginPage = new LoginPage(page);
        await loginPage.GotoAsync();
        await loginPage.SetEmail("administrator@localhost");
        await loginPage.SetPassword("Administrator1!");
        await loginPage.ClickLogin();
        await Assertions.Expect(page.Locator("a:has-text('Log out')")).ToBeVisibleAsync();

        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new TaskCapturePage(page));
    }

    [AfterFeature("TaskCapture")]
    public static async Task AfterTaskCaptureFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("an authenticated user visits the task capture page")]
    public Task GivenAnAuthenticatedUserVisitsTheTaskCapturePage()
        => taskCapturePage.GotoAsync();

    [When("the user captures a task using their own words")]
    public async Task WhenTheUserCapturesATaskUsingTheirOwnWords()
    {
        _originalInput = $"  Prepare the project kickoff notes {Guid.NewGuid():N}  ";

        await taskCapturePage.CaptureTaskAsync(_originalInput);
    }

    [Then("the original task input is shown unchanged on the task detail page")]
    public Task ThenTheOriginalTaskInputIsShownUnchangedOnTheTaskDetailPage()
        => taskCapturePage.AssertTaskDetailsAsync(_originalInput);

    [Then("a starting recommendation is shown")]
    public Task ThenAStartingRecommendationIsShown()
        => taskCapturePage.AssertStartPlanAsync();
}
