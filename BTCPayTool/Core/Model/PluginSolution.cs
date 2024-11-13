using BTCPayTool.Misc;
using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core.Model;

public class PluginSolution
{
    public PluginSolution(string root, string name, IGitClient gitClient, AppContext appContext)
    {
        Root = root;
        Name = name;
        GitClient = gitClient;
        AppContext = appContext;
    }

    public string Root { get; }
    public string Name { get; }
    public IGitClient GitClient { get; }
    public AppContext AppContext { get; }

    public async Task Initialize()
    {
        await InitRepo();
        await CreateSolution();
        await AddBtcPayServerSubmodule();
    }

    private Task InitRepo()
    {
        AppContext.Logger.LogInformation("Initializing repository...");

        Directory.CreateDirectory(Root);
        GitClient.Init();
        return Task.CompletedTask;
    }

    private async Task CreateSolution()
    {
        AppContext.Logger.LogInformation("Creating solution file...");

        var solutionName = Name + ".sln";
        if (File.Exists(solutionName))
        {
            throw new InvalidOperationException("Solution file already exists.");
        }

        var arguments = $"new sln --name {Name}";
        await AppContext.ProcessRunner.RunAsync(new ProcessSpec {Executable = "dotnet", Arguments = arguments.Split(" ").AsReadOnly()}, CancellationToken.None);
    }

    private async Task AddBtcPayServerSubmodule()
    {
        AppContext.Logger.LogInformation("Adding BTCPayServer submodule...");

        if (Directory.Exists("btcpayserver"))
        {
            throw new InvalidOperationException("Submodule already exists.");
        }

        await GitClient.AddSubmodule("btcpayserver", new Uri("https://github.com/btcpayserver/btcpayserver"));
        await AddBtcPayProjectsToSolution();
    }

    private async Task AddBtcPayProjectsToSolution()
    {
        var projectFiles = Directory.GetFiles(Root, "BTCPayServer*.csproj", SearchOption.AllDirectories);

        foreach (var projectFile in projectFiles)
        {
            await AppContext.SolutionHelper.AddProjectToSolution(projectFile);
        }
    }
}