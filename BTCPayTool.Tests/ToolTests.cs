using BTCPayTool.Core;
using BTCPayTool.Misc;
using FluentAssertions;
using Xunit.Abstractions;
using AppContext = BTCPayTool.Core.AppContext;

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
        var tool = GetTool();

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
        var tool = GetTool();

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
        var tool = GetTool();
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

    [Fact]
    public async Task New_plugin_fails_when_no_plugin_solution_initialized()
    {
        var tool = GetTool();
        using var session = new TestSession();

        var func = () => tool.NewPlugin(new NewPluginOptions()
        {
            Name = "MyPlugin",
        });

        await func.Should().ThrowAsync<InvalidOperationException>();
    }

    private Tool GetTool()
    {   
        // Setup dependencies
        var testLogger = new TestLogger(output);
        var processRunner = new ProcessRunner(Logger.GetLogger<ProcessRunner>());
        var solutionHelper = new SolutionHelper(processRunner);
        var appContext = new AppContext(testLogger, processRunner, solutionHelper);

        var tool = new Tool(appContext);

        return tool;
    }
}