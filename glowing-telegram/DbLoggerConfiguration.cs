
using Microsoft.Extensions.Logging;

namespace DbLogger.Logger;

public class DbLoggerConfiguration
{
    public int EventId { get; set; }
    public string ConnectionString { get; set; }
    public int MaxDays { get; set; } = -1;
    public string ServiceName { get; set; } = null;
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;
}