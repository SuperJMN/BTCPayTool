using BTCPayTool.Core;
using BTCPayTool.Misc;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;
using NewPluginOptions = BTCPayTool.Core.NewPluginOptions;

namespace BTCPayTool.Tests;

public class ToolTests
{
    private readonly ITestOutputHelper output;

    public ToolTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    [Fact]
    public async Task Initialize_plugin_solution_success()
    {
        var testLogger = new TestLogger(output);
        var tool = new Tool(testLogger);
        ProcessRunner.Instance = new ProcessRunner(testLogger);

        using var session = new TestSession();
        await tool.InitializePluginSolution(new InitializePluginSolutionOptions()
        {
            Name = "BestPlugins",
        });

        Directory.Exists("btcpayserver").Should().BeTrue();
        File.Exists(".gitmodules").Should().BeTrue();
        File.Exists("BestPlugins.sln").Should().BeTrue();
    }

    [Fact]
    public async Task Initialize_plugin_solution_fails_when_submodule_exists()
    {
        var tool = new Tool(NullLogger.Instance);

        using var ts = new TestSession();
        Directory.CreateDirectory("btcpayserver");

        var func = () => tool.InitializePluginSolution(new InitializePluginSolutionOptions()
        {
            Name = "BestPlugins",
        });

        await func.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task New_plugin_success()
    {
        var tool = new Tool(NullLogger.Instance);
        using var session = new TestSession();

        Directory.CreateDirectory("btcpayserver");
        File.Create(".gitmodules");
        File.Create("BestPlugins.sln");

        await tool.NewPlugin(new NewPluginOptions()
        {
            Name = "MyPlugin",
        });

        Directory.Exists(Path.Combine("Plugins", "MyPlugin")).Should().BeTrue();
    }
}