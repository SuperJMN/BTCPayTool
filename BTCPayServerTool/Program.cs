using System.Reflection;
using Serilog;

namespace BTCPayServerTool;

public static class Program
{

    public static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var creator = new PluginCreator(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MyPlugin");
        await creator.Create();
    }
}