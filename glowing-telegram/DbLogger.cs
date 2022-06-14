using System.Collections.Concurrent;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Dapper;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DbLogger.Logger;

public sealed class DbLogger : ILogger
{
    private readonly string _name;
    private readonly Func<DbLoggerConfiguration> _getCurrentConfig;

    public DbLogger(
        string name,
        Func<DbLoggerConfiguration> getCurrentConfig) =>
        (_name, _getCurrentConfig) = (name, getCurrentConfig);

    public IDisposable BeginScope<TState>(TState state) => default!;

    public bool IsEnabled(LogLevel logLevel)
    {
        var config = _getCurrentConfig();
        if (config.ConnectionString is null)
            return false;

        if (logLevel < config.MinimumLogLevel)
            return false;

        return _getCurrentConfig().ConnectionString is not null;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var config = _getCurrentConfig();

        if (config.EventId == 0 || config.EventId == eventId.Id)
        {
            using var conn = new SqlConnection(config.ConnectionString);

            var extra = exception switch
            {
                ApplicationException e => JsonConvert.SerializeObject(e.Extra),
                _ => null
            };

            var interestingException = exception switch
            {
                ApplicationException e => e.InnerException,
                Exception e => e,
                _ => null
            };

            var serviceName = config.ServiceName;

            conn.Execute("INSERT INTO Logs (Message, LogLevel, Extra, CreatedAt, StackTrace, ExceptionType, ServiceName) VALUES (@message, @logLevel, @extra, GETUTCDATE(), @stackTrace, @exceptionType, @serviceName)", new
            {
                message = formatter(state, interestingException),
                logLevel = logLevel.ToString(),
                extra,
                stackTrace = interestingException?.StackTrace,
                exceptionType = interestingException?.GetType().Name,
                serviceName,
            });

            if (config.MaxDays > 0)
            {
                conn.Execute("DELETE FROM Logs WHERE DATEDIFF(day, CreatedAt, GETUTCDATE()) > @maxDays", new { maxDays = config.MaxDays });
            }
        }
    }
}
