using System.IO.Compression;

namespace BTCPayTool.Core;

public static class ZipExtensions
{
    public static async Task ExtractDirectory(this ZipArchive zipArchive, string zipDirectoryPath, string destination)
    {
        try
        {
            var entriesToExtract = zipArchive.Entries
                .Where(x => x.FullName.StartsWith(zipDirectoryPath + "/") && x.Name != "");

            foreach (var entry in entriesToExtract)
            {
                var subPath = entry.FullName[zipDirectoryPath.Length..];
                var finalPath = Path.Combine(destination, subPath[1..]);

                Directory.CreateDirectory(Path.GetDirectoryName(finalPath) ?? throw new InvalidOperationException("Invalid path"));

                await using var fileStream = File.Open(finalPath, FileMode.Create, FileAccess.Write);
                await using var entryStream = entry.Open();
                await entryStream.CopyToAsync(fileStream);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to extract directory from ZIP archive", ex);
        }
    }
}