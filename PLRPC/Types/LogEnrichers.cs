using Serilog.Core;
using Serilog.Events;

namespace LBPUnion.PLRPC.Types;

public class LogEnrichers : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ProcessId", Environment.ProcessId));
    }
}