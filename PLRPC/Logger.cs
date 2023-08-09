using System.Drawing;
using System.Text;
using LBPUnion.PLRPC.Types.Logging;
using Pastel;
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

    /// <summary>
    /// Write debug information to the sink.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    public void Debug(string message, LogArea logArea)
    {
        this.logger.Debug(LoggerUtils.BuildMessage(message, logArea));
    }

    /// <summary>
    /// Write verbose information to the sink.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    public void Verbose(string message, LogArea logArea)
    {
        this.logger.Verbose(LoggerUtils.BuildMessage(message, logArea));
    }

    /// <summary>
    /// Write information to the sink.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    public void Information(string message, LogArea logArea)
    {
        this.logger.Information(LoggerUtils.BuildMessage(message, logArea));
    }

    /// <summary>
    /// Write a warning to the sink.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    public void Warning(string message, LogArea logArea)
    {
        this.logger.Warning(LoggerUtils.BuildMessage(message, logArea));
    }

    /// <summary>
    /// Write an error to the sink.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    public void Error(string message, LogArea logArea)
    {
        this.logger.Error(LoggerUtils.BuildMessage(message, logArea));
    }

    /// <summary>
    /// Write an error with an exception to the sink.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    /// <param name="exception">Exception object to be logged.</param>
    public void Error(string message, LogArea logArea, Exception exception)
    {
        this.logger.Error(exception, LoggerUtils.BuildMessage(message, logArea));
    }

    /// <summary>
    /// Write a fatal error to the sink.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    public void Fatal(string message, LogArea logArea)
    {
        this.logger.Fatal(LoggerUtils.BuildMessage(message, logArea));
    }

    /// <summary>
    /// Write a fatal error with an exception to the sink.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    /// <param name="exception">Exception object to be logged.</param>
    public void Fatal(string message, LogArea logArea, Exception exception)
    {
        this.logger.Fatal(exception, LoggerUtils.BuildMessage(message, logArea));
    }

    /// <summary>
    /// Write a log event to the sink manually.    
    /// </summary>
    /// <param name="logEvent">The log event to write.</param>
    public void Write(LogEvent logEvent)
    {
        this.logger.Write(logEvent);
    }
}

public abstract class LoggerUtils : Logger
{
    /// <summary>
    /// Specifies the max length to pad the log area string in the message builder.
    /// </summary>
    private static readonly int maxLogAreaLength = Enum.GetNames<LogArea>().Max(x => x.Length);

    /// <summary>
    /// Build a log message with the specified log area.
    /// </summary>
    /// <param name="message">String to be logged.</param>
    /// <param name="logArea">Log area to be referenced.</param>
    /// <returns>String containing formatted log message.</returns>
    public static string BuildMessage(string message, LogArea logArea)
    {
        StringBuilder sb = new();

        string formattedLogArea = logArea.ToString().PadRight(maxLogAreaLength).Pastel(Color.LightBlue);
        string formattedMessage = message.Pastel(Color.White);

        sb.Append($"[{formattedLogArea}] ".Pastel(Color.DimGray));
        sb.Append(formattedMessage);

        return sb.ToString();
    }
}