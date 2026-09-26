namespace FocusPocuss.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class RegistrationStepDefinitions(RegistrationPage registrationPage)
{
    [BeforeFeature("Registration")]
    public static async Task BeforeRegistrationFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.NewContextAsync(new BrowserNewContextOptions
        {
            Locale = "en-US"
        });
        var page = await context.NewPageAsync();

        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new RegistrationPage(page));
    }

    [AfterFeature("Registration")]
    public static async Task AfterRegistrationFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("a logged out user visits the registration page")]
    public Task GivenALoggedOutUserVisitsTheRegistrationPage()
        => registrationPage.GotoAsync();

    [When("the user submits registration with a weak password")]
    public Task WhenTheUserSubmitsRegistrationWithAWeakPassword()
        => registrationPage.RegisterAsync(
            $"weak-password-{Guid.NewGuid():N}@example.com",
            "abcdef");

    [Then("the complete password requirements are shown")]
    public Task ThenTheCompletePasswordRequirementsAreShown()
        => registrationPage.AssertPasswordRequirementsAsync();

    [When("the user registers with valid credentials")]
    public Task WhenTheUserRegistersWithValidCredentials()
        => registrationPage.RegisterAsync(
            $"registration-{Guid.NewGuid():N}@example.com",
            "Testing1234!");

    [Then("the login page is shown")]
    public Task ThenTheLoginPageIsShown()
        => registrationPage.AssertLoginPageAsync();

    [When("the user registers with an existing email address")]
    public async Task WhenTheUserRegistersWithAnExistingEmailAddress()
    {
        var email = $"existing-registration-{Guid.NewGuid():N}@example.com";

        await registrationPage.RegisterAsync(email, "Testing1234!");
        await registrationPage.AssertLoginPageAsync();
        await registrationPage.GotoAsync();
        await registrationPage.RegisterAsync(email, "Testing1234!");
    }

    [Then("an email already in use error is shown")]
    public Task ThenAnEmailAlreadyInUseErrorIsShown()
        => registrationPage.AssertEmailInUseAsync();
}
