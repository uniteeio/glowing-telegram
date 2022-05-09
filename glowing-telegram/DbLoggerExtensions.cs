
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace DbLogger.Logger;

public static class DbLoggerExtensions
{
    public static ILoggingBuilder AddDbLogger(
        this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, DbLoggerProvider>());

        LoggerProviderOptions.RegisterProviderOptions
            <DbLoggerConfiguration, DbLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddDbLogger(
        this ILoggingBuilder builder,
        Action<DbLoggerConfiguration> configure)
    {
        builder.AddDbLogger();
        builder.Services.Configure(configure);

        return builder;
    }
}