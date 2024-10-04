using BTCPayTool.Tests;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace BTCPayTool;

public class GitClient : IGitClient
{
    public ZafiroPath Path { get; }

    public GitClient(ZafiroPath path)
    {
        Path = path;
    }

    public Result AddSubmodule(string name, Uri uri)
    {
        return
            Result
                .Success()
                .BindIf(() => !ExistsSubmodule(name), () => GitWrapper.ExecuteGitCommand($"submodule add {uri} {name}", Path))
                .Bind(() => GitWrapper.ExecuteGitCommand("submodule init"))
                .Bind(() => GitWrapper.ExecuteGitCommand("submodule update"));
    }

    private bool ExistsSubmodule(string name)
    {
        return Directory.Exists(Path.Combine(name));
    }

    public Result Init()
    {
        return GitWrapper.ExecuteGitCommand("init", Path);
    }
}