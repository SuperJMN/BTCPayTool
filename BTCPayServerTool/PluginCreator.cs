using System.IO.Compression;
using Nuke.Common.IO;
using Nuke.Common.Tools.Git;
using Serilog;
using Task = System.Threading.Tasks.Task;

namespace BTCPayServerTool;

public class PluginCreator
{
    public string PluginName { get; }

    public PluginCreator(AbsolutePath root, string pluginName)
    {
        PluginName = pluginName;
        ServerPath = root / "btcpayserver";
        PluginPath = ServerPath / "BTCPayServer" / "Plugins" / pluginName;
    }

    public AbsolutePath PluginPath { get; }

    public AbsolutePath ServerPath { get; }

    public async Task Create()
    {
        CloneBtcPayServer();
        await AddPlugin();
        RenameFiles();
        ReplaceNames();
    }

    private void ReplaceNames()
    {
        Utils.ReplaceStringInFiles(ServerPath, "MyPlugin", PluginName);
    }

    private void RenameFiles()
    {
        Utils.ReplaceStringInFilenames(PluginPath, "MyPlugin", PluginName);
    }

    private async Task AddPlugin()
    {
        Log.Information("Adding plugin {Name}...", PluginName);
        
        if (Path.Exists(PluginPath))
        {
            Log.Warning("Plugin {Name} already exists at {Path}. Skipping.", PluginName, PluginPath);
            return;
        }
        
        var branch = "wip";
        var templateUri = $"https://github.com/superjmn/btcpayserver-plugin-template/archive/refs/heads/{branch}.zip";
        
        Log.Information("Fetching template from {Uri}", templateUri);
        await CopyTemplate(templateUri, branch);
        Log.Information("Plugin added");
    }

    private async Task CopyTemplate(string templateUri, string branch)
    {
        using var httpClient = new HttpClient();
        await using var streamAsync = await httpClient.GetStreamAsync(templateUri);
        var zipArchive = new ZipArchive(streamAsync, ZipArchiveMode.Read);

        var templatePath = $"btcpayserver-plugin-template-{branch}/MyPlugin/";
        var entriesToExtract = zipArchive.Entries.Where(x => x.FullName.StartsWith(templatePath) && x.Name != "");
        foreach (var entry in entriesToExtract)
        {
            var subPath = entry.FullName[templatePath.Length..];
            var finalPath = PluginPath / subPath;
            Directory.CreateDirectory(finalPath.Parent);
            await using var fileStream = File.Open(finalPath, FileMode.Create, FileAccess.Write);
            await using var entryStream = entry.Open();
            await entryStream.CopyToAsync(fileStream);
        }
    }

    private static void CloneBtcPayServer()
    {
        Log.Information("Cloning BTCPayServer...");

        if (Directory.Exists("btcpayserver"))
        {
            Log.Information("Folder already exist. Skipping");
            return;
        }

        Log.Information("Cloning BTCPayServer...");
        GitTasks.Git("clone https://github.com/btcpayserver/btcpayserver.git");
        Log.Information("BTCPayServer cloned successfully...");
    }
}