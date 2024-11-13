using System.CommandLine;
using BTCPayTool.Core;
using Microsoft.Extensions.Logging;
using BTCPayTool.Misc;
using AppContext = BTCPayTool.Core.AppContext;

namespace BTCPayTool;

public static class Program
{
    private static readonly ILogger Logger = Misc.Logger.GlobalLogger;

    public static async Task<int> Main(string[] args)
    {
        var tool = GetTool();
        var command = RootCommand(tool);

        return await command.InvokeAsync(args);
    }

    private static Tool GetTool()
    {
        var processRunner = new ProcessRunner(Misc.Logger.GetLogger<ProcessRunner>());
        var appContext = new AppContext(Logger, processRunner, new SolutionHelper(processRunner));
        var tool = new Tool(appContext);
        return tool;
    }

    private static RootCommand RootCommand(Tool tool)
    {
        var rootCommand = new RootCommand("BTCPayTool CLI tool");

        var newPluginCommand = new Command("new-plugin", "Creates a new plugin");
        var nameOption = new Option<string>("--name", "The name of the plugin") { IsRequired = true };
        newPluginCommand.AddOption(nameOption);

        newPluginCommand.SetHandler(async (string name) =>
        {
            await tool.NewPlugin(new NewPluginOptions { Name = name });
        }, nameOption);

        var initPluginCommand = new Command("init-plugin", "Initializes a new plugin solution");
        var solutionNameOption = new Option<string>("--name", "The name of the solution") { IsRequired = true };
        initPluginCommand.AddOption(solutionNameOption);

        initPluginCommand.SetHandler(async (string name) =>
        {
            await tool.InitializePluginSolution(new InitializePluginSolutionOptions { Name = name });
        }, solutionNameOption);

        rootCommand.AddCommand(newPluginCommand);
        rootCommand.AddCommand(initPluginCommand);
        return rootCommand;
    }
}