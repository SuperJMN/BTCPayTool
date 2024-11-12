using BTCPayTool.Core.Model;
using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core;

public class Tool(ILogger logger)
{
    public async Task InitializePluginSolution(InitializePluginSolutionOptions opts)
    {
        try
        {
            var outputDir = Directory.GetCurrentDirectory();
            var solution = new PluginSolution(outputDir, opts.Name, new GitClient(outputDir), logger);

            await solution.Initialize();

            logger.LogInformation("The plugin solution has been initialized.");
            logger.LogInformation("You can now add your first plugin by executing: btcpay new-plugin --name MyPlugin\"");
        }
        catch (Exception ex)
        {
            logger.LogError("Plugin solution initialization failed: {Error}", ex.Message);
            throw;
        }
    }

    public async Task NewPlugin(NewPluginOptions opts)
    {
        try
        {
            var outputDir = Directory.GetCurrentDirectory();
            var plugin = new Plugin(outputDir, opts.Name, new GitClient(outputDir), logger);

            var pluginPath = await plugin.Create();
            logger.LogInformation("The plugin has been added successfully! You can see it under {Path}", pluginPath);
        }
        catch (Exception ex)
        {
            logger.LogError("Plugin creation failed: {Error}", ex.Message);
            throw;
        }
    }
}