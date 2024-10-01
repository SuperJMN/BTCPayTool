using System.Reflection;
using Microsoft.Build.Utilities;
using Serilog;
using Task = System.Threading.Tasks.Task;

namespace BTCPayServerTool;

public static class Program
{

    public static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var creator = new PluginCreator(outputDir, "MyPlugin");
        await creator.Create();
        Log.Information("The plugin has been added successfully! You can see it under {Path}", creator.PluginPath);
    }
}