namespace FocusPocuss.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class HomeStepDefinitions(HomePage homePage)
{
    [BeforeFeature("Home")]
    public static async Task BeforeHomeFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.NewContextAsync(new() { Locale = "en-US" });
        var page = await context.NewPageAsync();
        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new HomePage(page));
    }

    [AfterFeature]
    public static async Task AfterHomeFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("a user visits the home page")]
    public Task GivenAUserVisitsTheHomePage() => homePage.GotoAsync();

    [Then("the heading {string} is visible")]
    public Task ThenTheHeadingIsVisible(string text) => homePage.AssertHeading(text);

    [When("the home language is changed to {string}")]
    public Task WhenTheHomeLanguageIsChanged(string language) => homePage.ChangeLanguage(language);

    [When("the system appearance is {string} and the selected theme is {string}")]
    public Task WhenTheThemeIsChanged(string system, string theme) => homePage.ChangeTheme(system, theme);

    [Then("the home canvas is {string} and the primary action is {string}")]
    public Task ThenTheHomeColorsAreApplied(string canvas, string primary) => homePage.AssertColors(canvas, primary);
}
