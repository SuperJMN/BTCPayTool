namespace BTCPayTool.Core.Model;

public class PluginCreator
{
    public PluginCreator(IGitClient gitClient, string root)
    {
        GitClient = gitClient;
        Root = root;
    }

    public IGitClient GitClient { get; }
    public string Root { get; }

    public Task<string> Create(string name)
    {
        return new Plugin(Root, name, GitClient).Create();
    }
}