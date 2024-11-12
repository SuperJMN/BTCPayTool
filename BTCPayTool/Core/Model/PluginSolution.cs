using BTCPayTool.Misc;
using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core.Model;

public class PluginSolution
{
    private readonly ILogger logger;

    public PluginSolution(string root, string name, IGitClient gitClient, ILogger logger)
    {
        this.logger = logger;
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
        logger.LogInformation("Initializing repository...");

        Directory.CreateDirectory(Root);
        GitClient.Init();
        return Task.CompletedTask;
    }

    private async Task CreateSolution()
    {
        logger.LogInformation("Creating solution file...");

        var solutionName = Name + ".sln";
        if (File.Exists(solutionName))
        {
            throw new InvalidOperationException("Solution file already exists.");
        }

        var arguments = $"new sln --name {Name}";
        await ProcessRunner.Instance.RunAsync(new ProcessSpec {Executable = "dotnet", Arguments = arguments.Split(" ").AsReadOnly()}, CancellationToken.None);
    }

    private async Task AddBtcPayServerSubmodule()
    {
        logger.LogInformation("Adding BTCPayServer submodule...");

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