namespace FocusPocuss.Web.AcceptanceTests.Pages;

public class RegistrationPage(IPage page) : BasePage(page)
{
    public override string PagePath => $"{BaseUrl}/register";

    public async Task RegisterAsync(string email, string password)
    {
        await Page.FillAsync("#email", email);
        await Page.FillAsync("#password", password);
        await Page.Locator("button[type='submit']").ClickAsync();
    }

    public Task AssertPasswordRequirementsAsync()
        => Assertions.Expect(Page.Locator("#password-helper"))
            .ToContainTextAsync("uppercase and lowercase letters, a number, and a special character");

    public Task AssertLoginPageAsync()
        => Assertions.Expect(Page).ToHaveURLAsync($"{BaseUrl}/login");

    public Task AssertEmailInUseAsync()
        => Assertions.Expect(Page.Locator(".error"))
            .ToHaveTextAsync("An account with this email already exists.");
}
