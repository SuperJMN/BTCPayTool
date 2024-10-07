namespace BTCPayTool.Core.Model;

public class PluginCreator
{
    public PluginCreator(IGitClient gitClient, ZafiroPath root)
    {
        GitClient = gitClient;
        Root = root;
    }

    public IGitClient GitClient { get; }
    public ZafiroPath Root { get; }

    public Task<Result<ZafiroPath>> Create(string name)
    {
        return new Plugin(Root, name, GitClient).Create();
    }
}