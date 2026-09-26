using System.Diagnostics;

namespace FocusPocuss.Web.AcceptanceTests;

[SetUpFixture]
public class PlaywrightSetup
{
    private const string ArtifactsDirectoryVariable = "PLAYWRIGHT_ARTIFACTS_DIR";
    private static bool IsHeadless => Debugger.IsAttached is false;
    private static IPlaywright? _playwright;

    public static IBrowser Browser { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Assertions.SetDefaultExpectTimeout(10_000);

        _playwright = await Playwright.CreateAsync();

        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = IsHeadless ? 0 : 500
        });
    }

    public static Task<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null)
    {
        options ??= new BrowserNewContextOptions();

        var artifactsDirectory = Environment.GetEnvironmentVariable(ArtifactsDirectoryVariable);
        if (!string.IsNullOrWhiteSpace(artifactsDirectory))
        {
            var videoDirectory = Path.Combine(artifactsDirectory, "videos");
            Directory.CreateDirectory(videoDirectory);
            options.RecordVideoDir = videoDirectory;
        }

        return Browser.NewContextAsync(options);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Browser.CloseAsync();
        _playwright?.Dispose();
    }
}
