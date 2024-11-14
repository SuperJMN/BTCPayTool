using BTCPayTool.Misc;
using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core;

public class AppContext
{
    public ILogger Logger { get; }
    public ProcessRunner ProcessRunner { get; }
    public SolutionHelper SolutionHelper { get; set; }

    public AppContext(ILogger logger, ProcessRunner processRunner, SolutionHelper solutionHelper)
    {
        Logger = logger;
        ProcessRunner = processRunner;
        SolutionHelper = solutionHelper;
    }
}

public class SolutionHelper
{
    public ProcessRunner ProcessRunner { get; }

    public SolutionHelper(ProcessRunner processRunner)
    {
        ProcessRunner = processRunner;
    }

  

    public async Task AddProjectToSolution(string projectPath)
    {
        await ProcessRunner.RunAsync(new ProcessSpec {Executable = "dotnet", Arguments = $"sln add {projectPath}".Split(" ").AsReadOnly()}, CancellationToken.None);
    }
}