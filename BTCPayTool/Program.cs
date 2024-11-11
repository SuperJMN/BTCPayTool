using BTCPayTool.Core;
using BTCPayTool.Core.Model;
using System.CommandLine;

namespace BTCPayTool
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var rootCommand = new RootCommand("BTCPayTool CLI tool");

            var newPluginCommand = new Command("new-plugin", "Creates a new plugin");
            var nameOption = new Option<string>("--name", "The name of the plugin") { IsRequired = true };
            newPluginCommand.AddOption(nameOption);

            newPluginCommand.SetHandler(async (string name) =>
            {
                await NewPlugin(new NewPluginOptions { Name = name });
            }, nameOption);

            var initPluginCommand = new Command("init-plugin", "Initializes a new plugin solution");
            var solutionNameOption = new Option<string>("--name", "The name of the solution") { IsRequired = true };
            initPluginCommand.AddOption(solutionNameOption);

            initPluginCommand.SetHandler(async (string name) =>
            {
                await InitializePluginSolution(new InitializePluginSolutionOptions { Name = name });
            }, solutionNameOption);

            rootCommand.AddCommand(newPluginCommand);
            rootCommand.AddCommand(initPluginCommand);

            return await rootCommand.InvokeAsync(args);
        }

        private static async Task InitializePluginSolution(InitializePluginSolutionOptions opts)
        {
            var outputDir = Directory.GetCurrentDirectory();
            var solution = new PluginSolution(outputDir, opts.Name, new GitClient(outputDir));

            var result = await solution.Initialize()
                .Tap(() => Log.Information("The plugin solution has been initialized. You can now add your first plugin by executing: btcpay new-plugin --name MyPlugin"))
                .TapError(error => Log.Error("Plugin creation failed: {Error}", error));

            result.Match(() => 0, _ => -1);
        }

        private static async Task NewPlugin(NewPluginOptions opts)
        {
            var outputDir = Directory.GetCurrentDirectory();
            var plugin = new Plugin(outputDir, opts.Name, new GitClient(outputDir));
            var result = await plugin.Create();

            result
                .Tap(pluginPath => Log.Information("The plugin has been added successfully! You can see it under {Path}", pluginPath))
                .TapError(error => Log.Error("Plugin creation failed: {Error}", error));

            result.Match(_ => 0, _ => -1);
        }
    }
}
