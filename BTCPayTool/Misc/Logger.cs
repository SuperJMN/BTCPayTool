using Microsoft.Extensions.Logging;

namespace BTCPayTool.Misc;

public static class Logger
{
    private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
    {
        builder.AddSimpleConsole(options =>
        {
            options.SingleLine = true;            
            options.TimestampFormat = "HH:mm:ss ";
            options.IncludeScopes = false;
        });
    });

    public static ILogger<T> GetLogger<T>() => LoggerFactory.CreateLogger<T>();

    public static ILogger GlobalLogger => LoggerFactory.CreateLogger("Global");
}