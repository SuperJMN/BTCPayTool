using BTCPayTool.Misc;

namespace BTCPayTool.Core;

public class GitClient : IGitClient
{
    public GitClient(string path, AppContext appContext)
    {
        Path = path;
        AppContext = appContext;
    }

    public string Path { get; }
    public AppContext AppContext { get; }

    public async Task AddSubmodule(string name, Uri uri)
    {
        if (!ExistsSubmodule(name))
        {
            var arguments = $"submodule add --branch master --depth 1 {uri} {name}".Split(" ").AsReadOnly();
            var result = await AppContext.ProcessRunner.RunAsync(
                new ProcessSpec {Executable = "git", Arguments = arguments, WorkingDirectory = Path},
                CancellationToken.None);

            if (result != 0)
            {
                throw new ApplicationException("Add submodule failed");
            }
        }

        await AppContext.ProcessRunner.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = "submodule init".Split(" ").AsReadOnly()},
            CancellationToken.None);
        await AppContext.ProcessRunner.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = "submodule update".Split(" ").AsReadOnly()},
            CancellationToken.None);
    }

    public async Task Init()
    {
        await AppContext.ProcessRunner.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = ["init"], WorkingDirectory = Path},
            CancellationToken.None);
    }

    private bool ExistsSubmodule(string name)
    {
        return Directory.Exists(System.IO.Path.Combine(Path, name));
    }
}