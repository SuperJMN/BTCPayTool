using BTCPayTool.Core.Model;
using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core;

public class Tool(AppContext appContext)
{
    public AppContext AppContext { get; } = appContext;

    public async Task InitializePluginSolution(InitializePluginSolutionOptions opts)
    {
        try
        {
            var outputDir = Directory.GetCurrentDirectory();
            var solution = new PluginSolution(outputDir, opts.Name, new GitClient(outputDir, AppContext), AppContext);

            await solution.Initialize();

            AppContext.Logger.LogInformation("The plugin solution has been initialized.");
            AppContext.Logger.LogInformation("You can now add your first plugin by executing: btcpay new-plugin --name MyPlugin\"");
        }
        catch (Exception ex)
        {
            AppContext.Logger.LogError("Plugin solution initialization failed: {Error}", ex.Message);
            throw;
        }
    }

    public async Task NewPlugin(NewPluginOptions opts)
    {
        if (!IsSolutionInitialized(opts.Name))
        {
            throw new InvalidOperationException(
                "The solution has not been initialized yet. Use 'btcpaytool init-plugin' first.");
        }

        try
        {
            var outputDir = Directory.GetCurrentDirectory();
            var plugin = new Plugin(outputDir, opts.Name, new GitClient(outputDir, AppContext), AppContext);

            var pluginPath = await plugin.Create();
            AppContext.Logger.LogInformation("The plugin has been added successfully! You can see it under {Path}", pluginPath);
        }
        catch (Exception ex)
        {
            AppContext.Logger.LogError("Plugin creation failed: {Error}", ex.Message);
            throw;
        }
    }

    private bool IsSolutionInitialized(string pluginName)
    {
        bool initialized = true;

        initialized = initialized && Directory.Exists("btcpayserver");
        initialized = initialized && File.Exists(".gitmodules");
        initialized = initialized && Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.sln").Any();

        return initialized;
    }
}