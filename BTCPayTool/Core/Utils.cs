namespace BTCPayTool.Core;

public static class Utils
{
    public static void ReplaceStringInFilenames(string directory, string toReplace, string replacement)
    {
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);

            if (fileName.Contains(toReplace))
            {
                var dir = Path.GetDirectoryName(file);
                var newFilename = fileName.Replace(toReplace, replacement);
                var newPath = Path.Combine(dir, newFilename);
                File.Move(file, newPath, true);
                Log.Debug("Renamed: {Old}, {New}", file, newPath);
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

    public static Result AddProjectToSolution(string projectPath)
    {
        return Result.Try(() => ProcessWrapper.Execute("dotnet", $"sln add {projectPath}"));
    }
}