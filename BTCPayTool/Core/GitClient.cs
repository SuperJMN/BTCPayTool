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
            var arguments = $"submodule add {uri} {name}";
            await (Task) ProcessRunner.Instance.RunAsync(
                new ProcessSpec {Executable = "git", Arguments = [arguments], WorkingDirectory = Path},
                CancellationToken.None);
        }

        await (Task) ProcessRunner.Instance.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = ["submodule init"]},
            CancellationToken.None);
        await (Task) ProcessRunner.Instance.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = ["submodule update"]},
            CancellationToken.None);
    }

    public async Task Init()
    {
        await (Task) ProcessRunner.Instance.RunAsync(
            new ProcessSpec {Executable = "git", Arguments = ["init"], WorkingDirectory = Path},
            CancellationToken.None);
    }

    private bool ExistsSubmodule(string name)
    {
        return Directory.Exists(System.IO.Path.Combine(Path, name));
    }
}