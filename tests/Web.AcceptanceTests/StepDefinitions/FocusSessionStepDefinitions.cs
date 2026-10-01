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
        var action = FocusSessionPage.ExpectedNextAction;
        string? savedThought = null;
        var endedEarly = false;
        var activeTaskId = 1;
        var activePlanId = 501;
        await page.RouteAsync("**/api/focus-sessions/active", route =>
        {
            using var request = System.Text.Json.JsonDocument.Parse(route.Request.PostData!);
            action = FocusSessionPage.ExpectedNextAction;
            savedThought = null;
            endedEarly = false;
            activeTaskId = request.RootElement.GetProperty("taskId").GetInt32();
            activePlanId = request.RootElement.GetProperty("taskStartPlanId").GetInt32();
            return FulfillSessionAsync(route, completed: false, taskId: activeTaskId, taskStartPlanId: activePlanId);
        });
        await page.RouteAsync($"**/api/focus-sessions/{FocusSessionPage.SessionId}", route =>
            FulfillSessionAsync(route, completed: false, taskId: activeTaskId, taskStartPlanId: activePlanId,
                action: action, thought: savedThought, endedEarly: endedEarly));
        await page.RouteAsync($"**/api/focus-sessions/{FocusSessionPage.SessionId}/complete", route =>
            FulfillSessionAsync(route, completed: true, taskId: activeTaskId, taskStartPlanId: activePlanId));
        await page.RouteAsync($"**/api/focus-sessions/{FocusSessionPage.SessionId}/reflection", route =>
            FulfillSessionAsync(route, completed: true, reflected: true, taskId: activeTaskId, taskStartPlanId: activePlanId));
        var reason = "UnclearNextAction";
        await page.RouteAsync($"**/api/focus-sessions/{FocusSessionPage.SessionId}/distractions", route =>
        {
            using var request = System.Text.Json.JsonDocument.Parse(route.Request.PostData!);
            reason = request.RootElement.GetProperty("reason").GetString()!;
            var type = reason switch
            {
                "TaskTooDifficult" => "ShrinkCurrentAction",
                "AnotherThought" => "ParkThought",
                "Tired" => "RecoveryChoice",
                "PhoneOrSocialMedia" => "EnvironmentalReset",
                _ => "ClarifyCurrentAction"
            };
            var requirement = reason switch
            {
                "AnotherThought" => "ThoughtCapture", "Tired" => "UserChoice", _ => "None"
            };
            return route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200, ContentType = "application/json",
                Body = System.Text.Json.JsonSerializer.Serialize(new
                {
                    id = 901, focusSessionId = FocusSessionPage.SessionId, reason,
                    strategy = "ClarifyNextAction",
                    intervention = new
                    {
                        type, version = "distraction-recovery-v1", currentAction = action,
                        returnAction = reason is "TaskTooDifficult" or "UnclearNextAction"
                            ? FocusSessionPage.ExpectedRecoveredAction : action,
                        requirement, choices = reason == "Tired"
                            ? new[] { "TakeShortReset", "ContinueWithSmallerAction", "EndSession" } : Array.Empty<string>()
                    }
                })
            });
        });
        await page.RouteAsync($"**/api/focus-sessions/{FocusSessionPage.SessionId}/distractions/*/resolve", route =>
        {
            using var request = System.Text.Json.JsonDocument.Parse(route.Request.PostData!);
            var resolution = request.RootElement.GetProperty("resolution").GetString();
            if (resolution == "EndSession") endedEarly = true;
            if (resolution == "ReturnToFocus")
            {
                if (reason == "AnotherThought") savedThought = request.RootElement.GetProperty("thought").GetString();
                else if (reason is "TaskTooDifficult" or "UnclearNextAction") action = FocusSessionPage.ExpectedRecoveredAction;
            }
            return FulfillSessionAsync(route, completed: false, taskId: activeTaskId, taskStartPlanId: activePlanId,
                action: action, thought: savedThought, endedEarly: endedEarly);
        });

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

    [Then("the focus session reflection question is shown")]
    public Task ThenTheFocusSessionReflectionQuestionIsShown()
        => focusSessionPage.AssertReflectionPromptAsync();

    [When("the user records that they focused well")]
    public Task WhenTheUserRecordsThatTheyFocusedWell()
        => focusSessionPage.RecordFocusedWellReflectionAsync();

    [When("the user skips the focus session reflection")]
    public Task WhenTheUserSkipsTheFocusSessionReflection()
        => focusSessionPage.SkipReflectionAsync();

    [Then("the focus session is shown as completed")]
    public Task ThenTheFocusSessionIsShownAsCompleted()
        => focusSessionPage.AssertCompletedAsync();

    [When("the user reports an unclear next action distraction")]
    public Task WhenTheUserReportsAnUnclearNextActionDistraction()
        => focusSessionPage.ReportUnclearNextActionDistractionAsync();

    [When("the user reports tiredness and the server selects clarification")]
    public Task WhenTheUserReportsTirednessAndTheServerSelectsClarification()
        => focusSessionPage.ReportTirednessWithServerClarificationAsync();

    [Then("the selected recovery guidance is shown")]
    public Task ThenTheSelectedRecoveryGuidanceIsShown()
        => focusSessionPage.AssertRecoveryGuidanceAsync();

    [When("the user returns to focus")]
    public Task WhenTheUserReturnsToFocus()
        => focusSessionPage.ReturnToFocusAsync();

    [Then("the recovery guidance is dismissed and the focus session remains active")]
    public Task ThenTheRecoveryGuidanceIsDismissedAndTheFocusSessionRemainsActive()
        => focusSessionPage.AssertRecoveryDismissedAndSessionActiveAsync();

    [Then("starting with an open session for a different {string} requires explicit navigation")]
    public Task ThenStartingWithAnOpenSessionRequiresExplicitNavigation(string scope)
        => focusSessionPage.AssertExistingSessionRequiresExplicitNavigationAsync(scope);

    [Then("one clarification can be supplied on mobile after reload")]
    public Task ThenClarificationWorksOnMobile() => focusSessionPage.ClarifyOnMobileAfterReloadAsync();

    [When("the user parks a thought and returns")]
    public Task WhenTheUserParksAThought() => focusSessionPage.ParkThoughtAsync();

    [Then("the saved thought survives reload without changing the action")]
    public Task ThenTheThoughtSurvivesReload() => focusSessionPage.AssertParkedThoughtAfterReloadAsync();

    [When("the tired user chooses to end the session")]
    public Task WhenTheTiredUserEnds() => focusSessionPage.EndEarlyAsync();

    [Then("the session ends without a completion celebration")]
    public Task ThenTheSessionEndsEarly() => focusSessionPage.AssertEndedEarlyAsync();

    private static Task FulfillSessionAsync(
        IRoute route,
        bool completed,
        bool reflected = false,
        int taskId = 1,
        int taskStartPlanId = 501,
        string action = FocusSessionPage.ExpectedNextAction,
        string? thought = null, bool endedEarly = false)
    {
        var completedAtUtc = completed ? "\"2026-09-25T08:20:00Z\"" : "null";
        var reflection = reflected ? "\"FocusedWell\"" : "null";
        var reflectedAtUtc = reflected ? "\"2026-09-25T08:21:00Z\"" : "null";

        return route.FulfillAsync(new RouteFulfillOptions
        {
            Status = 200,
            ContentType = "application/json",
            Body = $$"""
                {
                  "id": {{FocusSessionPage.SessionId}},
                  "taskId": {{taskId}},
                  "taskStartPlanId": {{taskStartPlanId}},
                  "action": {{System.Text.Json.JsonSerializer.Serialize(action)}},
                  "endedEarlyAtUtc": {{(endedEarly ? "\"2026-10-01T08:20:00Z\"" : "null")}},
                  "parkedThoughts": {{System.Text.Json.JsonSerializer.Serialize(thought is null ? Array.Empty<object>() : new object[] { new { id = 901, text = thought, savedAtUtc = "2026-10-01T08:20:00Z" } })}},
                  "plannedDurationMinutes": 10,
                  "startedAtUtc": "2020-01-01T08:00:00Z",
                  "completedAtUtc": {{completedAtUtc}},
                  "reflection": {{reflection}},
                  "reflectedAtUtc": {{reflectedAtUtc}}
                }
                """
        });
    }
}
