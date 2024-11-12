using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core.Model;

public class PluginCreator
{
    private readonly ILogger logger;

    public PluginCreator(IGitClient gitClient, string root, ILogger logger)
    {
        this.logger = logger;
        GitClient = gitClient;
        Root = root;
    }

    public IGitClient GitClient { get; }
    public string Root { get; }

    public Task<string> Create(string name)
    {
        return new Plugin(Root, name, GitClient, logger).Create();
    }
}