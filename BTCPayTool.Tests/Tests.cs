using BTCPayTool.Core;
using BTCPayTool.Core.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace BTCPayTool.Tests;

public class Tests
{
    [Fact]
    public async Task Test1()
    {
        var newPluginCreator = new PluginCreator(new GitClient("Output"), "Output", NullLogger.Instance);
        var result = await newPluginCreator.Create("MyPlugin");
        result.Should().NotBe(null);
    }
}