using System.Text;
using LBPUnion.PLRPC.Types.Logging;
using Serilog;
using Serilog.Events;

namespace LBPUnion.PLRPC;

// ReSharper disable TemplateIsNotCompileTimeConstantProblem
// ReSharper disable UnusedMember.Global
public class Logger
{
    private static readonly LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.With<LogEnricher>()
        .WriteTo.Console(outputTemplate: "[{ProcessId} {ThreadId} {Timestamp:HH:mm:ss} {Level:u3}] {Message:l}{NewLine}{Exception}");
    
    private readonly ILogger logger = loggerConfiguration.CreateLogger();

    /*
     * Used for generating a universal logging template
     */

    private static string BuildLogMessage(string message, LogArea logArea)
    {
        StringBuilder sb = new();

        sb.Append($"[{logArea.ToString().PadRight(20)}] {message}");

        return sb.ToString();
    }

    /*
     * Log levels that cannot have embedded exceptions
     */

    public void Debug(string message, LogArea logArea)
    {
        this.logger.Debug(BuildLogMessage(message, logArea));
    }

    public void Verbose(string message, LogArea logArea)
    {
        this.logger.Verbose(BuildLogMessage(message, logArea));
    }

    public void Information(string message, LogArea logArea)
    {
        this.logger.Information(BuildLogMessage(message, logArea));
    }

    public void Warning(string message, LogArea logArea)
    {
        this.logger.Warning(BuildLogMessage(message, logArea));
    }

    /*
     * Log levels that can have embedded exceptions
     */

    public void Error(string message, LogArea logArea)
    {
        this.logger.Error(BuildLogMessage(message, logArea));
    }

    public void Error(string message, LogArea logArea, Exception exception)
    {
        this.logger.Error(exception, BuildLogMessage(message, logArea));
    }

    public void Fatal(string message, LogArea logArea)
    {
        this.logger.Fatal(BuildLogMessage(message, logArea));
    }

    public void Fatal(string message, LogArea logArea, Exception exception)
    {
        this.logger.Fatal(exception, BuildLogMessage(message, logArea));
    }

    /*
     * Manually write a log event
     */

    public void Write(LogEvent logEvent)
    {
        this.logger.Write(logEvent);
    }
}