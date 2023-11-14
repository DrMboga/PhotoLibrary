using System.Diagnostics;

namespace PhotoLibraryBackend.MediaReader;

public static class BashHelper
{
    public static Task<int> Bash(this string cmd, ILogger logger)
    {
        var source = new TaskCompletionSource<int>();
        var escapedArgs = cmd.Replace("\"", "\\\"");
        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                                        {
                                            FileName = "bash",
                                            Arguments = $"-c \"{escapedArgs}\"",
                                            RedirectStandardOutput = true,
                                            RedirectStandardError = true,
                                            UseShellExecute = false,
                                            CreateNoWindow = true
                                        },
                            EnableRaisingEvents = true
                        };
        process.Exited += (sender, args) =>
            {
                string errorMessage = process.StandardError.ReadToEnd();
                logger.LogWarning(errorMessage);
                logger.LogInformation(process.StandardOutput.ReadToEnd());
                if (process.ExitCode == 0)
                {
                    source.SetResult(0);
                }
                else
                {
                    source.SetException(new ShellExecuteException(process.ExitCode, errorMessage));
                }
                process.Dispose();
            };

        try
        {
            process.Start();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Command {} failed", cmd);
            source.SetException(e);
        }

        return source.Task;
    }
}
