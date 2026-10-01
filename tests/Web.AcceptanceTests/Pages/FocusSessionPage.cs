using System.Text.RegularExpressions;

namespace FocusPocuss.Web.AcceptanceTests.Pages;

public class FocusSessionPage(IPage page) : BasePage(page)
{
    public const int SessionId = 73;
    public const string ExpectedRecoveredAction = "Write the first review note.";
    public const string ExpectedNextAction = "Open the project and write the first review note.";

    public override string PagePath => $"{BaseUrl}/focus/{SessionId}";

    public async Task StartAsync()
    {
        await Page.Locator(".focus-start-button").ClickAsync();
        await Assertions.Expect(Page).ToHaveURLAsync(new Regex($@"/focus/{SessionId}$"));
    }

    public async Task AssertFocusedActionWithoutNavigationAsync()
    {
        await Assertions.Expect(Page.Locator("#focus-action"))
            .ToHaveTextAsync(ExpectedNextAction);
        await Assertions.Expect(Page.Locator("nav")).ToHaveCountAsync(0);
        await Assertions.Expect(Page.Locator("footer")).ToHaveCountAsync(0);
    }

    public async Task AssertExpiredSessionRemainsActiveAsync()
    {
        await Assertions.Expect(Page.Locator(".focus-session-timer time"))
            .ToHaveTextAsync("00:00");
        await Assertions.Expect(Page.Locator(".focus-session-expired"))
            .ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator(".focus-session button:has-text('This step is done')"))
            .ToHaveTextAsync("This step is done");

        // Hiding the readout must not change expiration or completion availability.
        await Page.GetByRole(AriaRole.Button, new() { Name = "Hide timer", Exact = true }).ClickAsync();
        await Assertions.Expect(Page.Locator("#focus-remaining-time")).ToHaveCountAsync(0);
        await Assertions.Expect(Page.Locator(".focus-session-expired")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Show timer", Exact = true }).ClickAsync();
        await Assertions.Expect(Page.Locator("#focus-remaining-time")).ToHaveTextAsync("00:00");
    }

    public Task CompleteAsync()
        => Page.Locator(".focus-session button:has-text('This step is done')").ClickAsync();

    public async Task AssertReflectionPromptAsync()
    {
        await Assertions.Expect(Page.Locator("#focus-reflection-title"))
            .ToHaveTextAsync("How did it go?");
        await Assertions.Expect(Page.Locator(".focus-reflection-options button"))
            .ToHaveCountAsync(3);
    }

    public async Task RecordFocusedWellReflectionAsync()
    {
        var request = await Page.RunAndWaitForRequestAsync(
            () => Page.Locator(".focus-reflection-options button:has-text('I focused well')")
                .ClickAsync(),
            request => request.Method == "PUT"
                && request.Url.EndsWith($"/api/focus-sessions/{SessionId}/reflection"));

        request.PostData.ShouldBe("{\"reflection\":\"FocusedWell\"}");
    }

    public async Task SkipReflectionAsync()
    {
        var requestCountBeforeSkip = await GetReflectionRequestCountAsync();

        await Page.Locator(".focus-reflection-skip:has-text('Skip for now')")
            .ClickAsync();
        await AssertCompletedAsync();

        var requestCountAfterSkip = await GetReflectionRequestCountAsync();
        requestCountAfterSkip.ShouldBe(requestCountBeforeSkip);
    }

    public Task AssertCompletedAsync()
        => Assertions.Expect(Page.Locator("#focus-completed-title"))
            .ToHaveTextAsync("You moved forward.");

    public async Task ReportUnclearNextActionDistractionAsync()
    {
        await Page.Locator(".focus-distraction-trigger").ClickAsync();
        await Assertions.Expect(Page.Locator(".focus-recovery-dialog")).ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator(".focus-reason-options input")).ToHaveCountAsync(6);
        foreach (var reason in new[] { "TaskTooDifficult", "UnclearNextAction", "PhoneOrSocialMedia", "AnotherThought", "Tired", "Other" })
        {
            await Assertions.Expect(Page.Locator($".focus-reason-options input[value='{reason}']"))
                .ToHaveCountAsync(1);
        }

        await Page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(Page.Locator(".focus-recovery-dialog")).Not.ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator(".focus-distraction-trigger")).ToBeFocusedAsync();
        await Page.Locator(".focus-distraction-trigger").ClickAsync();
        await Page.GetByLabel("The next action is unclear").CheckAsync();
        var request = await Page.RunAndWaitForRequestAsync(
            () => Page.Locator(".focus-distraction-form button[type='submit']").ClickAsync(),
            request => request.Method == "POST"
                && request.Url.EndsWith($"/api/focus-sessions/{SessionId}/distractions"));
        request.PostData.ShouldBe("{\"reason\":\"UnclearNextAction\",\"language\":\"en\"}");
    }

    public async Task ReportTirednessWithServerClarificationAsync()
    {
        var url = $"**/api/focus-sessions/{SessionId}/distractions";
        // Deliberately return a different strategy to prove the UI follows the response,
        // rather than inferring an intervention from the submitted reason.
        Func<IRoute, Task> handler = route => route.FulfillAsync(new RouteFulfillOptions
        {
            Status = 200,
            ContentType = "application/json",
            Body = System.Text.Json.JsonSerializer.Serialize(new
            {
                id = 902, focusSessionId = SessionId, reason = "Tired", strategy = "ClarifyNextAction",
                intervention = new
                {
                    type = "ClarifyCurrentAction", version = "distraction-recovery-v1",
                    currentAction = ExpectedNextAction, returnAction = ExpectedRecoveredAction,
                    requirement = "None", choices = Array.Empty<string>()
                }
            })
        });
        await Page.RouteAsync(url, handler);
        try
        {
            await Page.Locator(".focus-distraction-trigger").ClickAsync();
            await Page.GetByLabel("I'm tired", new() { Exact = true }).CheckAsync();
            var request = await Page.RunAndWaitForRequestAsync(
                () => Page.Locator(".focus-distraction-form button[type='submit']").ClickAsync(),
                request => request.Method == "POST" && request.Url.EndsWith($"/api/focus-sessions/{SessionId}/distractions"));
            request.PostData.ShouldBe("{\"reason\":\"Tired\",\"language\":\"en\"}");
            await AssertRecoveryGuidanceAsync();
        }
        finally
        {
            await Page.UnrouteAsync(url, handler);
        }
    }

    public async Task AssertRecoveryGuidanceAsync()
    {
        await Assertions.Expect(Page.Locator(".focus-distraction-success"))
            .ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator(".focus-distraction-success-title"))
            .ToHaveTextAsync("Remove the uncertainty first.");
        await Assertions.Expect(Page.Locator(".focus-distraction-message"))
            .ToHaveTextAsync("The action you will work on when you return:");
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
        await Assertions.Expect(Page.Locator(".focus-distraction-trigger"))
            .ToBeFocusedAsync();
        await Assertions.Expect(Page.Locator("#focus-action"))
            .ToHaveTextAsync(ExpectedRecoveredAction);
        await Assertions.Expect(Page.Locator(".focus-session button:has-text('This step is done')"))
            .ToBeVisibleAsync();
    }

    public async Task AssertExistingSessionRequiresExplicitNavigationAsync(string scope)
    {
        const string action = "Continue the earlier task.";
        var taskUrl = Page.Url;
        var startUrl = "**/api/focus-sessions/active";
        var detailUrl = $"**/api/focus-sessions/{SessionId}";
        var response = "";
        Func<IRoute, Task> startHandler = route =>
        {
            using var request = System.Text.Json.JsonDocument.Parse(route.Request.PostData!);
            var taskId = request.RootElement.GetProperty("taskId").GetInt32();
            var planId = request.RootElement.GetProperty("taskStartPlanId").GetInt32();
            response = System.Text.Json.JsonSerializer.Serialize(new
            {
                id = SessionId,
                taskId = scope == "task" ? taskId + 1 : taskId,
                taskStartPlanId = scope == "plan" ? planId + 1 : planId,
                action,
                plannedDurationMinutes = 10,
                startedAtUtc = "2020-01-01T08:00:00Z"
            });
            return route.FulfillAsync(new() { Status = 200, ContentType = "application/json", Body = response });
        };
        Func<IRoute, Task> detailHandler = route => route.FulfillAsync(new()
        {
            Status = 200, ContentType = "application/json", Body = response
        });
        await Page.RouteAsync(startUrl, startHandler);
        await Page.RouteAsync(detailUrl, detailHandler);
        try
        {
            await Page.Locator(".focus-start-button").ClickAsync();
            await Assertions.Expect(Page.Locator(".focus-existing-session")).ToBeVisibleAsync();
            await Assertions.Expect(Page).ToHaveURLAsync(taskUrl);
            await Assertions.Expect(Page.Locator(".focus-existing-session-action")).ToHaveTextAsync(action);
            await Page.GetByRole(AriaRole.Link, new() { Name = "Return to the open session" }).ClickAsync();
            await Assertions.Expect(Page).ToHaveURLAsync(new Regex($@"/focus/{SessionId}$"));
            await Assertions.Expect(Page.Locator("#focus-action")).ToHaveTextAsync(action);
        }
        finally
        {
            await Page.UnrouteAsync(startUrl, startHandler);
            await Page.UnrouteAsync(detailUrl, detailHandler);
        }
    }

    public async Task ClarifyOnMobileAfterReloadAsync()
    {
        var requirement = "NeedsClarification";
        object Report() => new
        {
            id = 903, focusSessionId = SessionId, reason = "UnclearNextAction", strategy = "ClarifyNextAction",
            clarificationUsed = requirement == "None",
            intervention = new
            {
                type = "ClarifyCurrentAction", version = "distraction-recovery-v1", currentAction = ExpectedNextAction,
                returnAction = requirement == "None" ? ExpectedRecoveredAction : null,
                requirement, choices = Array.Empty<string>()
            }
        };
        var reportUrl = $"**/api/focus-sessions/{SessionId}/distractions";
        var detailUrl = $"**/api/focus-sessions/{SessionId}";
        var prepareUrl = $"**/api/focus-sessions/{SessionId}/distractions/903/prepare";
        Func<IRoute, Task> reportHandler = route => route.FulfillAsync(new()
        {
            Status = 200, ContentType = "application/json", Body = System.Text.Json.JsonSerializer.Serialize(Report())
        });
        Func<IRoute, Task> detailHandler = route => route.FulfillAsync(new()
        {
            Status = 200, ContentType = "application/json", Body = System.Text.Json.JsonSerializer.Serialize(new
            {
                id = SessionId, taskId = 1, taskStartPlanId = 501, action = ExpectedNextAction,
                plannedDurationMinutes = 10, startedAtUtc = "2026-10-01T08:00:00Z", pendingRecovery = Report()
            })
        });
        Func<IRoute, Task> prepareHandler = route =>
        {
            using var request = System.Text.Json.JsonDocument.Parse(route.Request.PostData!);
            request.RootElement.GetProperty("clarification").GetString().ShouldBe("The first review note is about the visible login error.");
            requirement = "None";
            return reportHandler(route);
        };
        await Page.RouteAsync(reportUrl, reportHandler);
        await Page.RouteAsync(detailUrl, detailHandler);
        await Page.RouteAsync(prepareUrl, prepareHandler);
        try
        {
            await Page.SetViewportSizeAsync(390, 844);
            await ReportUnclearNextActionDistractionAsync();
            await Assertions.Expect(Page.Locator("#recovery-input")).ToBeVisibleAsync();
            await Page.ReloadAsync();
            await Assertions.Expect(Page.Locator("#recovery-input")).ToBeVisibleAsync();
            await Page.GetByLabel("Where are you stuck? Write a short detail.")
                .FillAsync("The first review note is about the visible login error.");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Clarify the action", Exact = true }).ClickAsync();
            await Assertions.Expect(Page.Locator(".focus-recovery-action p")).ToHaveTextAsync(ExpectedRecoveredAction);
            await Assertions.Expect(Page.Locator("#recovery-input")).ToHaveCountAsync(0);
            (await Page.EvaluateAsync<bool>("document.documentElement.scrollWidth <= window.innerWidth")).ShouldBeTrue();
            var artifacts = Environment.GetEnvironmentVariable("PLAYWRIGHT_ARTIFACTS_DIR");
            if (!string.IsNullOrWhiteSpace(artifacts))
                await Page.ScreenshotAsync(new() { Path = Path.Combine(artifacts, "recovery-mobile.png"), FullPage = true });
        }
        finally
        {
            await Page.UnrouteAsync(reportUrl, reportHandler);
            await Page.UnrouteAsync(detailUrl, detailHandler);
            await Page.UnrouteAsync(prepareUrl, prepareHandler);
            await Page.SetViewportSizeAsync(1280, 720);
        }
    }

    public async Task ParkThoughtAsync()
    {
        await Page.Locator(".focus-distraction-trigger").ClickAsync();
        await Page.GetByLabel("Another thought pulled me away", new() { Exact = true }).CheckAsync();
        await Page.Locator(".focus-distraction-form button[type='submit']").ClickAsync();
        await Page.GetByLabel("The thought you want to remember later").FillAsync("Call Deniz tomorrow.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and return", Exact = true }).ClickAsync();
        await Assertions.Expect(Page.Locator(".focus-recovery-dialog")).Not.ToBeVisibleAsync();
    }

    public async Task AssertParkedThoughtAfterReloadAsync()
    {
        await Page.ReloadAsync();
        await Assertions.Expect(Page.Locator("#focus-action")).ToHaveTextAsync(ExpectedNextAction);
        await Page.Locator(".focus-parked-thoughts summary").ClickAsync();
        await Assertions.Expect(Page.Locator(".focus-parked-thoughts li")).ToHaveTextAsync("Call Deniz tomorrow.");
    }

    public async Task EndEarlyAsync()
    {
        await Page.Locator(".focus-distraction-trigger").ClickAsync();
        await Page.GetByLabel("I'm tired", new() { Exact = true }).CheckAsync();
        await Page.Locator(".focus-distraction-form button[type='submit']").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "End the session here", Exact = true }).ClickAsync();
    }

    public async Task AssertEndedEarlyAsync()
    {
        await Assertions.Expect(Page.Locator("#focus-completed-title")).ToHaveTextAsync("The session ended here.");
        await Assertions.Expect(Page.Locator(".focus-reflection")).ToHaveCountAsync(0);
        await Page.ReloadAsync();
        await Assertions.Expect(Page.Locator("#focus-completed-title")).ToHaveTextAsync("The session ended here.");
    }

    private async Task<int> GetReflectionRequestCountAsync()
    {
        var requests = await Page.RequestsAsync();

        return requests.Count(request => request.Method == "PUT"
            && request.Url.EndsWith($"/api/focus-sessions/{SessionId}/reflection"));
    }
}
