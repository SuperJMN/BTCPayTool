using BTCPayTool.Core;
using BTCPayTool.Core.Model;
using System.CommandLine;
using Microsoft.Extensions.Logging;
using BTCPayTool.Misc;

namespace BTCPayTool
{
    public static class Program
    {
        private static readonly ILogger Logger = Misc.Logger.GlobalLogger;

        public static async Task<int> Main(string[] args)
        {
            ProcessRunner.Instance = new ProcessRunner(Misc.Logger.GetLogger<ProcessRunner>());

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
            try
            {
                var outputDir = Directory.GetCurrentDirectory();
                var solution = new PluginSolution(outputDir, opts.Name, new GitClient(outputDir), Logger);

                await solution.Initialize();

                Logger.LogInformation("The plugin solution has been initialized.");
                Logger.LogInformation("You can now add your first plugin by executing: btcpay new-plugin --name MyPlugin\"");
            }
            catch (Exception ex)
            {
                Logger.LogError("Plugin solution initialization failed: {Error}", ex.Message);
                Environment.Exit(-1);
            }
        }

        private static async Task NewPlugin(NewPluginOptions opts)
        {
            try
            {
                var outputDir = Directory.GetCurrentDirectory();
                var plugin = new Plugin(outputDir, opts.Name, new GitClient(outputDir), Logger);

                var pluginPath = await plugin.Create();
                Logger.LogInformation("The plugin has been added successfully! You can see it under {Path}", pluginPath);
            }
            catch (Exception ex)
            {
                Logger.LogError("Plugin creation failed: {Error}", ex.Message);
                Environment.Exit(-1);
            }
        }
    }
}