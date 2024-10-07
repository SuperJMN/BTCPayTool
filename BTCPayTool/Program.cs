using BTCPayTool.Core;
using BTCPayTool.Core.Model;
using CommandLine;

namespace BTCPayTool;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var mapResult = await Parser.Default.ParseArguments<NewPluginOptions, InitializePluginSolutionOptions>(args)
            .MapResult<NewPluginOptions, InitializePluginSolutionOptions, Task<int>>(
                NewPlugin,
                InitializePluginSolution, 
                errors => Task.FromResult(HandleParseError(errors.ToList())));

        return mapResult;
    }

    private static async Task<int> InitializePluginSolution(InitializePluginSolutionOptions opts)
    {
        var outputDir = Directory.GetCurrentDirectory();
        var solution = new PluginSolution(outputDir, opts.Name, new GitClient(outputDir));

        var result = await solution.Initialize()
            .Tap(() => Log.Information("The plugin solution has been initialized. You can now add your first plugin by executing: btcpay new-plugin --name MyPlugin"))
            .TapError(error => Log.Error("Plugin creation failed: {Error}", error));

        return result.Match(() => 0, _ => -1);
    }

    private static async Task<int> NewPlugin(NewPluginOptions opts)
    {
        var outputDir = Directory.GetCurrentDirectory();
        var plugin = new Plugin(outputDir, opts.Name, new GitClient(outputDir));
        var result = await plugin.Create();

        result
            .Tap(pluginPath => Log.Information("The plugin has been added successfully! You can see it under {Path}", pluginPath))
            .TapError(error => Log.Error("Plugin creation failed: {Error}", error));

        return result.Match(_ => 0, _ => -1);
    }

    private static int HandleParseError(ICollection<Error> errors)
    {
        var result = -2;

        if (errors.Any(x => x is HelpRequestedError || x is VersionRequestedError || x is HelpVerbRequestedError))
        {
            result = -1;
        }

        return result;
    }
}