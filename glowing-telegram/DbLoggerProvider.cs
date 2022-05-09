using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DbLogger.Logger;

[ProviderAlias("DbLogger")]
public sealed class DbLoggerProvider : ILoggerProvider
{
    private readonly IDisposable _onChangeToken;
    private DbLoggerConfiguration _currentConfig;

    private readonly ConcurrentDictionary<string, DbLogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public DbLoggerProvider(
        IOptionsMonitor<DbLoggerConfiguration> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new DbLogger(name, GetCurrentConfig));

    private DbLoggerConfiguration GetCurrentConfig() => _currentConfig;

    public void Dispose()
    {
        _loggers.Clear();
        _onChangeToken.Dispose();
    }
}