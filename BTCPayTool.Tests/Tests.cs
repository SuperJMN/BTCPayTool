using BTCPayTool.Core;
using BTCPayTool.Core.Model;
using FluentAssertions;

namespace BTCPayTool.Tests;

public class Tests
{
    [Fact]
    public async Task Test1()
    {
        var newPluginCreator = new PluginCreator(new GitClient("Output"), "Output");
        var result = await newPluginCreator.Create("MyPlugin");
        result.Should().Succeed();
    }
}