namespace BTCPayTool.Core;

public class ReplaceUtils
{
    public static void ReplaceStringInFilenames(string directory, string toReplace, string replacement)
    {
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);

            if (fileName.Contains(toReplace))
            {
                var dir = Path.GetDirectoryName(file) ?? throw new InvalidOperationException("Invalid directory path");
                var newFilename = fileName.Replace(toReplace, replacement);
                var newPath = Path.Combine(dir, newFilename);

                File.Move(file, newPath, true);
            }
        }
    }

    public static void ReplaceStringInFiles(string directory, string toReplace, string replacement)
    {
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            if (content.Contains(toReplace))
            {
                var newContent = content.Replace(toReplace, replacement);
                File.WriteAllText(file, newContent);
            }
        }
    }
}