using System.IO.Compression;
using BTCPayTool.Misc;
using Microsoft.Extensions.Logging;

namespace BTCPayTool.Core.Model;

public class PluginTemplateProject(string name, ILogger logger)
{
    public string Name { get; } = name;

    public async Task CopyTo(string directory)
    {
        await CreatePluginFromTemplate(directory);
        RenameTemplateFiles(directory);
        ReplaceTextInTemplateFiles(directory);
    }

    private async Task CreatePluginFromTemplate(string directory)
    {
        Directory.CreateDirectory(directory);

        var branch = "wip";
        var templateUri = $"https://github.com/superjmn/btcpayserver-plugin-template/archive/refs/heads/{branch}.zip";

        logger.LogInformation("Fetching template from {Uri}", templateUri);

        await ExtractTemplate(templateUri, branch, directory);

        logger.LogInformation("Plugin added");
    }

    private void ReplaceTextInTemplateFiles(string directory)
    {
        Utils.ReplaceStringInFiles(directory, "MyPlugin", Name);
    }

    private void RenameTemplateFiles(string directory)
    {
        Utils.ReplaceStringInFilenames(directory, "MyPlugin", Name, logger);
    }

    private async Task ExtractTemplate(string templateUri, string branch, string directory)
    {
        var templatePath = $"btcpayserver-plugin-template-{branch}/MyPlugin";

        using var httpClient = new HttpClient();
        await using var stream = await httpClient.GetStreamAsync(templateUri);
        
        using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
        await zipArchive.ExtractDirectory(templatePath, directory);
    }
}