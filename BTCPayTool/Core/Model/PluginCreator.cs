using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core.Model;

public class PluginCreator
{
    public PluginCreator(IGitClient gitClient, string root, AppContext appContext)
    {
        GitClient = gitClient;
        Root = root;
        AppContext = appContext;
    }

    public IGitClient GitClient { get; }
    public string Root { get; }
    public AppContext AppContext { get; }

    public Task<string> Create(string name)
    {
        return new Plugin(Root, name, GitClient, AppContext).Create();
    }
}