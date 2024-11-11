using BTCPayTool.Misc;
using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core.Model;

public class PluginSolution
{
    public PluginSolution(string root, string name, IGitClient gitClient)
    {
        Root = root;
        Name = name;
        GitClient = gitClient;
    }

    public string Root { get; }
    public string Name { get; }
    public IGitClient GitClient { get; }

    public async Task Initialize()
    {
        await InitRepo();
        await CreateSolution();
        await AddBtcPayServerSubmodule();
    }

    private Task InitRepo()
    {
        Logger.GlobalLogger.LogInformation("Initializing repository...");

        Directory.CreateDirectory(Root);
        GitClient.Init();
        return Task.CompletedTask;
    }

    private async Task CreateSolution()
    {
        Logger.GlobalLogger.LogInformation("Creating solution file...");

        var solutionName = Name + ".sln";
        if (File.Exists(solutionName))
        {
            throw new InvalidOperationException("Solution file already exists.");
        }

        var arguments = $"new sln --name {Name}";
        await ProcessRunner.Instance.RunAsync(new ProcessSpec {Executable = "dotnet", Arguments = [arguments]}, CancellationToken.None);
    }

    private async Task AddBtcPayServerSubmodule()
    {
        Logger.GlobalLogger.LogInformation("Adding BTCPayServer submodule...");

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
            await Utils.AddProjectToSolution(projectFile);
        }
    }
}