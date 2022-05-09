
namespace DbLogger.Logger;

public class DbLoggerConfiguration
{
    public int EventId { get; set; }
    public string ConnectionString { get; set; }
    public int MaxDays { get; set; } = -1;
}