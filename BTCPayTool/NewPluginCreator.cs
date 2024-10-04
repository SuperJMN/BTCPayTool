using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace BTCPayTool;

public class NewPluginCreator
{
    public IGitClient GitClient { get; }

    public NewPluginCreator(IGitClient gitClient)
    {
        GitClient = gitClient;
    }
    
    public Task<Result<ZafiroPath>> Create(string path, string name)
    {
        var pluginDeployment = new PluginDeployment(path, name, GitClient);
        return pluginDeployment.Deploy();
    }
}