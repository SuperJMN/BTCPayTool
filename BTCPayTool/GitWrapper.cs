using System.Diagnostics;
using CSharpFunctionalExtensions;

namespace BTCPayTool.Tests;

public class GitWrapper
{
    public static Result ExecuteGitCommand(string arguments, string workingDirectory = "")
    {
        try
        {
            // Configuramos el proceso para ejecutar Git
            var processInfo = new ProcessStartInfo("git", arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? Environment.CurrentDirectory : workingDirectory
            };

            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();

                // Capturamos la salida estándar y de error
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    return Result.Success(output.Trim()); // Comando exitoso
                }
                else
                {
                    return Result.Failure($"Error al ejecutar el comando Git: {error.Trim()}");
                }
            }
        }
        catch (Exception ex)
        {
            return Result.Failure($"Excepción al ejecutar el comando Git: {ex.Message}");
        }
    }
}