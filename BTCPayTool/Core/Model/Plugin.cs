namespace BTCPayTool.Core.Model;

public class Plugin
{
    public Plugin(string root, string name, IGitClient gitClient)
    {
        Root = root;
        Name = name;
        PluginRoot = Path.Combine(root, "Plugins", name);
        GitClient = gitClient;
    }

    public string Root { get; }
    public string Name { get; }

    public IGitClient GitClient { get; }

    public string PluginRoot { get; }

    public async Task<Result<string>> Create()
    {
        Log.Information("Adding plugin {Name}...", Name);

        if (Path.Exists(PluginRoot))
        {
            return Result.Failure<string>($"Plugin {Name} already exists");
        }

        return await AddPluginCore().Bind(AddPluginProjectToSolution).Map(() => PluginRoot);
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