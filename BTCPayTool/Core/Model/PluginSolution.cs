namespace BTCPayTool.Core.Model;

public class PluginSolution
{
    public string Root { get; }
    public string Name { get; }
    public IGitClient GitClient { get; }

    public PluginSolution(string root, string name, IGitClient gitClient)
    {
        Root = root;
        Name = name;
        GitClient = gitClient;
    }
    
    public Task<Result> Initialize()
    {
        return Result.Success()
            .Bind(InitRepo)
            .Bind(CreateSolution)
            .Bind(AddBtcPayServerSubmodule);
    }
    
    private async Task<Result> InitRepo()
    {
        Log.Information("Initializing repository...");

        return Result.Try(() => Directory.CreateDirectory(Root))
            .Bind(_ => GitClient.Init());
    }
    
    private async Task<Result> CreateSolution()
    {
        Log.Information("Creating solution file...");
        
        var solutionName = Name + ".sln";
        if (File.Exists(solutionName))
        {
            return Result.Failure("Solution file already exists");
        }
        
        return Result.Try(() => ProcessWrapper.Execute("dotnet", $"new sln --name {Name}"));
    }
    
    private async Task<Result> AddBtcPayServerSubmodule()
    {
        Log.Information("Adding BTCPayServer submodule...");

        if (Directory.Exists("btcpayserver"))
        {
            return Result.Failure("Submodule already exists.");
        }

        return GitClient.AddSubmodule("btcpayserver", new Uri("https://github.com/btcpayserver/btcpayserver"))
            .Bind(AddBtcPayProjectsToSolution);
    }

    private Result AddBtcPayProjectsToSolution()
    {
        return Result.Try(() => Directory.GetFiles(Root, "BTCPayServer*.csproj", SearchOption.AllDirectories))
            .Bind(strings => strings.Select(Utils.AddProjectToSolution).Combine());
    }
    
   
}