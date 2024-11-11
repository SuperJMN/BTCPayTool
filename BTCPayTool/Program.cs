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

            // Define el comando raíz y agrega descripciones generales
            var rootCommand = new RootCommand("BTCPayTool CLI tool");

            // Comando para crear un nuevo plugin
            var newPluginCommand = new Command("new-plugin", "Creates a new plugin");
            var nameOption = new Option<string>("--name", "The name of the plugin") { IsRequired = true };
            newPluginCommand.AddOption(nameOption);

            newPluginCommand.SetHandler(async (string name) =>
            {
                await NewPlugin(new NewPluginOptions { Name = name });
            }, nameOption);

            // Comando para inicializar una solución de plugins
            var initPluginCommand = new Command("init-plugin", "Initializes a new plugin solution");
            var solutionNameOption = new Option<string>("--name", "The name of the solution") { IsRequired = true };
            initPluginCommand.AddOption(solutionNameOption);

            initPluginCommand.SetHandler(async (string name) =>
            {
                await InitializePluginSolution(new InitializePluginSolutionOptions { Name = name });
            }, solutionNameOption);

            // Agrega los comandos al comando raíz
            rootCommand.AddCommand(newPluginCommand);
            rootCommand.AddCommand(initPluginCommand);

            // Ejecuta el comando raíz
            return await rootCommand.InvokeAsync(args);
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
    }
}
