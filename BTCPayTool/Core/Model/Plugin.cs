using BTCPayTool.Misc;
using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core.Model;

public class Plugin
{
    private readonly ILogger logger;

    public Plugin(string root, string name, IGitClient gitClient, ILogger logger)
    {
        this.logger = logger;
        Root = root;
        Name = name;
        PluginRoot = Path.Combine(root, "Plugins", name);
        GitClient = gitClient;
    }

    public string Root { get; }
    public string Name { get; }
    public IGitClient GitClient { get; }
    public string PluginRoot { get; }

    public async Task<string> Create()
    {
        logger.LogInformation("Adding plugin {Name}...", Name);

        if (Directory.Exists(PluginRoot))
        {
            throw new InvalidOperationException($"Plugin {Name} already exists");
        }

        await AddPluginCore();
        AddPluginProjectToSolution();

        return PluginRoot;
    }

    private async Task AddPluginCore()
    {
        Directory.CreateDirectory(PluginRoot);
        await new PluginTemplateProject(Name, logger).CopyTo(PluginRoot);
        RenameTemplateFiles();
        ReplaceTextInTemplateFiles();
    }

    private void AddPluginProjectToSolution()
    {
        logger.LogInformation("Adding plugin to solution...");

        var projectFiles = Directory.GetFiles(PluginRoot, "*.csproj");
        if (!projectFiles.Any())
        {
            throw new FileNotFoundException("Cannot find project file.");
        }

        var projectFile = projectFiles.First();
        Utils.AddProjectToSolution(projectFile);
    }

    private void RenameTemplateFiles()
    {
        Utils.ReplaceStringInFilenames(PluginRoot, "MyPlugin", Name, logger);
    }

    private void ReplaceTextInTemplateFiles()
    {
        Utils.ReplaceStringInFiles(PluginRoot, "MyPlugin", Name);
    }
}