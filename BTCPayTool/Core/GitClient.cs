using BTCPayTool.Misc;

namespace BTCPayTool.Core;

public class GitClient : IGitClient
{
    public GitClient(string path)
    {
        Path = path;
    }

    public string Path { get; }

    public async Task AddSubmodule(string name, Uri uri)
    {
        if (!ExistsSubmodule(name))
        {
            var arguments = $"submodule add {uri} {name}".Split(" ").AsReadOnly();
            var result = await ProcessRunner.Instance.RunAsync(
                new ProcessSpec {Executable = "git", Arguments = arguments, WorkingDirectory = Path},
                CancellationToken.None);

            if (result != 0)
            {
                throw new ApplicationException("Add submodule failed");
            }
        }

        await ProcessRunner.Instance.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = "submodule init".Split(" ").AsReadOnly()},
            CancellationToken.None);
        await ProcessRunner.Instance.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = "submodule update".Split(" ").AsReadOnly()},
            CancellationToken.None);
    }

    public async Task Init()
    {
        await ProcessRunner.Instance.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = ["init"], WorkingDirectory = Path},
            CancellationToken.None);
    }

    private bool ExistsSubmodule(string name)
    {
        return Directory.Exists(System.IO.Path.Combine(Path, name));
    }
}