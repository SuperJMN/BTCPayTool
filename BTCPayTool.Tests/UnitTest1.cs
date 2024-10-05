using BTCPayTool.Core;
using FluentAssertions;

namespace BTCPayTool.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var newPluginCreator = new PluginCreator(new GitClient("Output"), "Output");
        var result = await newPluginCreator.Create("MyPlugin");
        result.Should().Succeed();
    }
}