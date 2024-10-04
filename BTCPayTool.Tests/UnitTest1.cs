using FluentAssertions;

namespace BTCPayTool.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var newPluginCreator = new NewPluginCreator(new GitClient("Output"));
        var result = await newPluginCreator.Create("Output", "MyPlugin");
        result.Should().Succeed();
    }
}