using Microsoft.AspNetCore.Http;

namespace DbLogger.Logger;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    readonly Func<HttpContext, object> _getExtraFromContext;

    public ErrorHandlerMiddleware(RequestDelegate next, Func<HttpContext, object> getExtraFromContext = null)
    {
        _next = next;

        if (getExtraFromContext is null)
        {
            _getExtraFromContext = DefaultImplGetExtraFromContext;
        }
        else
        {
            _getExtraFromContext = getExtraFromContext;
        }
    }

    public Func<HttpContext, object> GetExtraFromContext => _getExtraFromContext;

    public static object DefaultImplGetExtraFromContext(HttpContext context)
    {
        return new
        {
            headers = context.Request.Headers.ToDictionary(x => x.Key, y => y.Value.FirstOrDefault()),
            claims = context.User.Claims.ToDictionary(x => x.Type, y => y.Value),
        };
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var extra = GetExtraFromContext(context);
            throw new ApplicationException(error.Message, error, extra);
        }
    }
}