using BTCPayTool.Core;
using BTCPayTool.Core.Operations;
using CommandLine;

namespace BTCPayTool;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var mapResult = await Parser.Default.ParseArguments<AddPluginOptions, InitializePluginSolutionOptions>(args)
            .MapResult<AddPluginOptions, InitializePluginSolutionOptions, Task<int>>(
                addPlugin => AddPlugin(addPlugin),
                initializePluginSolutionOptions => InitializePluginSolutionOptions(initializePluginSolutionOptions), 
                errors => Task.FromResult(-1));

        return mapResult;
    }

    private static async Task<int> InitializePluginSolutionOptions(InitializePluginSolutionOptions opts)
    {
        var outputDir = Directory.GetCurrentDirectory();
        var solution = new PluginSolution(outputDir, opts.Name, new GitClient(outputDir));

        var result = await solution.Initialize()
            .Tap(() => Log.Information("The plugin solution has been initialized. You can now add your first plugin by executing: btcpay add-new-plugin --name MyPlugin"))
            .TapError(error => Log.Error("Plugin creation failed: {Error}", error));

        return result.Match(() => 0, _ => -1);
    }

    private static async Task<int> AddPlugin(AddPluginOptions opts)
    {
        var outputDir = Directory.GetCurrentDirectory();
        var plugin = new Plugin(outputDir, opts.Name, new GitClient(outputDir));
        var result = await plugin.Create();

        result
            .Tap(pluginPath => Log.Information("The plugin has been added successfully! You can see it under {Path}", pluginPath))
            .TapError(error => Log.Error("Plugin creation failed: {Error}", error));

        return result.Match(_ => 0, _ => -1);
    }
}