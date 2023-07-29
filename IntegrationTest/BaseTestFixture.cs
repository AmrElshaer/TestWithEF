namespace IntegrationTest;

using static Testing;

[TestFixture]
public abstract class BaseTestFixture
{
    protected HttpClient Client { get => Testing.Client; }

    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }
}
