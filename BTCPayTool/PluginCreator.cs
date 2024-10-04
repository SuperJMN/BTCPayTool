using System.IO.Compression;
using CSharpFunctionalExtensions;
using Nuke.Common.IO;
using Nuke.Common.Tools.Git;
using Serilog;
using Zafiro.FileSystem.Core;

namespace BTCPayTool;

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
        return new PluginDeployment(Root, name, GitClient).Deploy();
    }
}