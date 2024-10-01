using System.IO.Compression;
using Nuke.Common.IO;
using Nuke.Common.Tools.Git;
using Serilog;

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

    public AbsolutePath PluginPath { get; set; }

    public AbsolutePath ServerPath { get; }

    public async Task Create()
    {
        CloneBtcPayServer();
        await AddPlugin();
    }

    private async Task AddPlugin()
    {
        var branch = "wip";
        var streamAsync = await new HttpClient().GetStreamAsync($"https://github.com/superjmn/btcpayserver-plugin-template/archive/refs/heads/{branch}.zip");
        var zipArchive = new ZipArchive(streamAsync, ZipArchiveMode.Read);

        var templatePath = $"btcpayserver-plugin-template-{branch}/BTCPayServer.Plugins.MyPlugin/";
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