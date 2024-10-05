namespace BTCPayTool.Core.Operations;

public class Plugin
{
    public Plugin(ZafiroPath root, string name, IGitClient gitClient)
    {
        Root = root;
        Name = name;
        PluginRoot = root.Combine("Plugins").Combine(name);
        GitClient = gitClient;
    }

    public ZafiroPath Root { get; }
    public string Name { get; }

    public IGitClient GitClient { get; }

    public ZafiroPath PluginRoot { get; }

    public Task<Result<ZafiroPath>> Create()
    {
        return AddPlugin().Map(() => PluginRoot);
    }

    private async Task<Result> AddPlugin()
    {
        Log.Information("Adding plugin {Name}...", Name);

        if (Path.Exists(PluginRoot))
        {
            return Result.Failure($"Plugin {Name} already exists");
        }

        return await AddPluginCore().Bind(AddPluginProjectToSolution);
    }

    private Result AddPluginProjectToSolution()
    {
        Log.Information("Adding plugin to solution...");

        var projectResult = Result.Try(() => Directory.GetFiles(PluginRoot, "*.csproj")).Bind(strings => strings.TryFirst().ToResult("Cannot find project file."));
        
        return projectResult
            .Bind(Utils.AddProjectToSolution);
    }

    private Task<Result> AddPluginCore()
    {
        return Result.Try(() => Directory.CreateDirectory(PluginRoot))
            .Bind(_ => new PluginTemplateProject(Name).CopyTo(PluginRoot))
            .Bind(RenameTemplateFiles)
            .Bind(ReplaceTextInTemplateFiles);
    }

    private Result ReplaceTextInTemplateFiles()
    {
        return Result.Try(() => Utils.ReplaceStringInFiles(PluginRoot, "MyPlugin", Name));
    }

    private Result RenameTemplateFiles()
    {
        return Result.Try(() => Utils.ReplaceStringInFilenames(PluginRoot, "MyPlugin", Name));
    }
}