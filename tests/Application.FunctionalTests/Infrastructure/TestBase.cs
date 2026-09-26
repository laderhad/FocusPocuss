namespace FocusPocuss.Application.FunctionalTests.Infrastructure;

public abstract class TestBase
{
    protected static readonly TimeSpan DatabaseTimestampPrecision = TimeSpan.FromMicroseconds(1);

    [SetUp]
    public async Task SetUp()
    {
        await TestApp.ResetState();
    }
}
