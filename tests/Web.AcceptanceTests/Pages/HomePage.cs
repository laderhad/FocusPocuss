namespace FocusPocuss.Web.AcceptanceTests.Pages;

public class HomePage(IPage page) : BasePage(page)
{
    public override string PagePath => BaseUrl;

    public Task AssertHeading(string text)
        => Assertions.Expect(Page.Locator("h1")).ToHaveTextAsync(text);

    public Task ChangeLanguage(string language)
        => Page.Locator(".language-select").SelectOptionAsync(language);

    public async Task ChangeTheme(string system, string theme)
    {
        await Page.EmulateMediaAsync(new() { ColorScheme = system == "dark" ? ColorScheme.Dark : ColorScheme.Light });

        for (var attempt = 0; attempt < 3; attempt++)
        {
            var currentTheme = await Page.Locator("html").GetAttributeAsync("data-theme") ?? "auto";
            if (currentTheme == theme) return;
            await Page.Locator(".theme-toggle-btn").ClickAsync();
        }

        Assert.Fail($"Could not select theme '{theme}'.");
    }

    public async Task AssertColors(string canvas, string primary)
    {
        await Assertions.Expect(Page.Locator("html")).ToHaveCSSAsync("background-color", canvas);
        await Assertions.Expect(Page.Locator(".home-intro [role='button']")).ToHaveCSSAsync("background-color", primary);
    }
}
