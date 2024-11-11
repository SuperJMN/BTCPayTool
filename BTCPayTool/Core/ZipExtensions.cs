using System.IO.Compression;

namespace BTCPayTool.Core;

public static class ZipExtensions
{
    public static Task<Result> ExtractDirectory(this ZipArchive zipArchive, string zipDirectoryPath, string destination)
    {
        return Result.Try(async () =>
        {
            var entriesToExtract = zipArchive.Entries.Where(x => x.FullName.StartsWith(zipDirectoryPath + "/") && x.Name != "");
            foreach (var entry in entriesToExtract)
            {
                var subPath = entry.FullName[zipDirectoryPath.Length..];
                var finalPath = Path.Combine(destination, subPath[1..]);
                Directory.CreateDirectory(Path.GetDirectoryName(finalPath));
                await using var fileStream = File.Open(finalPath, FileMode.Create, FileAccess.Write);
                await using var entryStream = entry.Open();
                await entryStream.CopyToAsync(fileStream);
            }
        });
    }
}