using System.Reflection;
using CommandLine;
using Microsoft.Build.Utilities;
using Serilog;
using Task = System.Threading.Tasks.Task;

namespace BTCPayServerTool;

[Verb("new-plugin", HelpText = "Crear un nuevo plugin.")]
class NewPluginOptions
{
    [Option("name", Default = "MyPlugin", HelpText = "Plugin name")]
    public string Name { get; set; }
}

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
        var outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var creator = new PluginCreator(outputDir, opts.Name);
        await creator.Create();
        Log.Information("The plugin has been added successfully! You can see it under {Path}", creator.PluginPath);
        return 0;
    }
}