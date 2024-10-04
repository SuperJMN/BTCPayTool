using System.IO.Compression;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace BTCPayTool;

public static class ZipExtensions
{
    public static Task<Result> ExtractDirectory(this ZipArchive zipArchive, ZafiroPath zipDirectoryPath, ZafiroPath destination)
    {
        return Result.Try(async () =>
        {
            var entriesToExtract = zipArchive.Entries.Where(x => x.FullName.StartsWith(zipDirectoryPath + "/") && x.Name != "");
            foreach (var entry in entriesToExtract)
            {
                var subPath = entry.FullName[zipDirectoryPath.ToString().Length..];
                var finalPath = destination.Combine(subPath);
                Directory.CreateDirectory(finalPath.Parent().Value);
                await using var fileStream = File.Open(finalPath, FileMode.Create, FileAccess.Write);
                await using var entryStream = entry.Open();
                await entryStream.CopyToAsync(fileStream);
            }
        });
    }
}