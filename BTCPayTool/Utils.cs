using Serilog;

namespace BTCPayTool;

public static class Utils
{
    public static void ReplaceStringInFilenames(string directory, string toReplace, string replacement)
    {
        string[] files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            if (fileName.Contains(toReplace))
            {
                var dir = Path.GetDirectoryName(file);
                var newFilename = fileName.Replace(toReplace, replacement);
                var newPath = Path.Combine(dir, newFilename);
                File.Move(file, newPath);
                Log.Debug("Renamed: {Old}, {New}", file, newPath);
            }
        }
    }
    
    public static void ReplaceStringInFiles(string directory, string toReplace, string replacement)
    {
        string[] files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string content = File.ReadAllText(file);
            if (content.Contains(toReplace))
            {
                string newContent = content.Replace(toReplace, replacement);
                File.WriteAllText(file, newContent);
            }
        }
    }

}