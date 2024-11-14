using System.Runtime.CompilerServices;

namespace BTCPayTool.Tests;

public class TestSession : IDisposable
{
    private readonly string testDirectory;
    private readonly string previousDir;

    public TestSession([CallerMemberName]string directoryName = "")
    {
        previousDir = Directory.GetCurrentDirectory();
        testDirectory = Path.Combine("Tests", directoryName);
        Directory.CreateDirectory(testDirectory);
        Directory.SetCurrentDirectory(testDirectory);
    }

    public string TestDirectory => testDirectory;

    public void Dispose()
    {
        Directory.SetCurrentDirectory(previousDir);

        bool deleted = false; 
        var attempts = 0;

        while (!deleted && attempts < 3)
        {
            try
            {
                ForceDeleteDirectory(TestDirectory);
                
                deleted = !Directory.Exists(TestDirectory);
            }
            catch (Exception)
            {
                // ignored
            }
            
            if (!deleted)
            {
                Thread.Sleep(500);
            }

            attempts++;
        }
    }


    public static void ForceDeleteDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            foreach (var file in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(file);
                fileInfo.Attributes = FileAttributes.Normal; 
                fileInfo.Delete();
            }

            Directory.Delete(directoryPath, true);
        }
    }
}