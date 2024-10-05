using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Core;

namespace BTCPayTool;

public class PluginDeployment
{
    public ZafiroPath Root { get; }
    public string Name { get; }

    public IGitClient GitClient { get; }
    
    public PluginDeployment(ZafiroPath root, string name, IGitClient gitClient)
    {
        Root = root;
        Name = name;
        PluginRoot = root.Combine("Plugins").Combine(name);
        GitClient = gitClient;
    }

    public ZafiroPath PluginRoot { get; }

    public Task<Result<ZafiroPath>> Deploy()
    {
        return
            InitRepo()
                .Bind(AddBtcPayServerSubmodule)
                .Bind(TryAddPlugin)
                .Map(() => PluginRoot);
    }

    public Task<Result> TryAddPlugin()
    {
        Log.Information("Adding plugin {Name}...", Name);

        return Result
            .Success()
            .BindIf(() => !Path.Exists(PluginRoot).TapTrue(() => Log.Warning("Plugin {Name} already exists at {Path}. Skipping.", Name, PluginRoot)), AddPluginCore);
    }

    private Task<Result> AddPluginCore()
    {
        return Result.Try(() => Directory.CreateDirectory(PluginRoot))
            .Bind(_ => new PluginTemplateProject(Name).CopyTo(PluginRoot))
            .Bind(RenameTemplateFiles)
            .Bind(ReplaceTextInTemplateFiles);
    }

    private async Task<Result> InitRepo()
    {
        Log.Information("Initializing repo...");
        
        return Result.Try(() => Directory.CreateDirectory(Root))
            .Bind(_ => GitClient.Init());
    }
    
    private Result ReplaceTextInTemplateFiles()
    {
        return Result.Try(() => Utils.ReplaceStringInFiles(PluginRoot, "MyPlugin", Name));
    }

    private Result RenameTemplateFiles()
    {
        return Result.Try(() => Utils.ReplaceStringInFilenames(PluginRoot, "MyPlugin", Name));
    }

    private async Task<Result> AddBtcPayServerSubmodule()
    {
        Log.Information("Adding BTCPayServer Submodule...");
        return GitClient.AddSubmodule("btcpayserver", new Uri("https://github.com/btcpayserver/btcpayserver"));
    }
}