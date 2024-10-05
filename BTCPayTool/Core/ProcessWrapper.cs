using System.Diagnostics;

namespace BTCPayTool.Core;

public class ProcessWrapper
{
    public static Result Execute(string command, string arguments, string workingDirectory = "")
    {
        try
        {
            // Configuramos el proceso para ejecutar Git
            var processInfo = new ProcessStartInfo(command, arguments)
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
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    return Result.Success(output.Trim()); // Comando exitoso
                }

                return Result.Failure($"Error al ejecutar el comando Git: {error.Trim()}");
            }
        }
        catch (Exception ex)
        {
            return Result.Failure($"Excepción al ejecutar el comando Git: {ex.Message}");
        }
    }
}