using System.IO.Compression;
using CSharpFunctionalExtensions;
using Serilog;

namespace BTCPayTool;

public class PluginTemplateProject
{
    public string Name { get; }

    public PluginTemplateProject(string name)
    {
        Name = name;
    }
    
    public Task<Result> CopyTo(string directory)
    {
        return Result.Success()
            .Bind(() => CreatePluginFromTemplate(directory))
            .Bind(() => RenameTemplateFiles(directory))
            .Bind(() => ReplaceTextInTemplateFiles(directory));
    }
    
    private async Task<Result> CreatePluginFromTemplate(string directory)
    {
        Directory.CreateDirectory(directory);
        
        var branch = "wip";
        var templateUri = $"https://github.com/superjmn/btcpayserver-plugin-template/archive/refs/heads/{branch}.zip";
        
        Log.Information("Fetching template from {Uri}", templateUri);
        var result = await ExtractTemplate(templateUri, branch, directory);
        Log.Information("Plugin added");
        return result;
    }
    
    private Result ReplaceTextInTemplateFiles(string directory)
    {
        return Result.Try(() => Utils.ReplaceStringInFiles(directory, "MyPlugin", Name));
    }

    private Result RenameTemplateFiles(string directory)
    {
        return Result.Try(() => Utils.ReplaceStringInFilenames(directory, "MyPlugin", Name));
    }

    private Task<Result> ExtractTemplate(string templateUri, string branch, string directory)
    {
        var templatePath = $"btcpayserver-plugin-template-{branch}/MyPlugin";
        
        return ResultExtensions.Using(() =>
        {
            return Result.Try(async () =>
            {
                using var httpClient = new HttpClient();
                await using var streamAsync = await httpClient.GetStreamAsync(templateUri);
                var zipArchive = new ZipArchive(streamAsync, ZipArchiveMode.Read);
                return zipArchive;
            });
        }, archive => archive.ExtractDirectory(templatePath, directory));
    }
}