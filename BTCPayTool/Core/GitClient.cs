namespace BTCPayTool.Core;

public class GitClient : IGitClient
{
    public GitClient(ZafiroPath path)
    {
        Path = path;
    }

    public ZafiroPath Path { get; }

    public Result AddSubmodule(string name, Uri uri)
    {
        return
            Result
                .Success()
                .BindIf(() => !ExistsSubmodule(name), () => ProcessWrapper.Execute("git", $"submodule add {uri} {name}", Path))
                .Bind(() => ProcessWrapper.Execute("git", "submodule init"))
                .Bind(() => ProcessWrapper.Execute("git", "submodule update"));
    }

    public Result Init()
    {
        return ProcessWrapper.Execute("git", "init", Path);
    }

    private bool ExistsSubmodule(string name)
    {
        return Directory.Exists(Path.Combine(name));
    }
}