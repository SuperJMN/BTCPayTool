using CommandLine;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;

namespace BTCPayTool;

public static class Program
{
    public static async Task<int> Main(string [] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var mapResult = await Parser.Default.ParseArguments<NewPluginOptions>(args)
            .MapResult(ExecuteNewPlugin, errs => Task.FromResult(-1));
        
        return mapResult;
    }

    private static async Task<int> ExecuteNewPlugin(NewPluginOptions opts)
    {
        var outputDir = Directory.GetCurrentDirectory();
        var creator = new PluginCreator(new GitClient(outputDir), outputDir);
        var result = await creator.Create(opts.Name);
        
        result
            .Tap(pluginPath => Log.Information("The plugin has been added successfully! You can see it under {Path}", pluginPath))
            .TapError(error =>  Log.Error("Plugin creation failed: {Error}", error));
        
        return result.Match(_ => 0, _ => -1);
    }
}