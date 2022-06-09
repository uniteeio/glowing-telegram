# DbLogger

ILogger that logs into a mssql database.

```
$ dotnet add package glowing-telegram
```

## How to use
Add the logger to the `Program.cs`.

```cs
builder.Logging.ClearProviders();
builder.Logging.AddDbLogger(configuration =>
{
    configuration.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
});
```

## Configuration

```cs
configuration.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
configuration.MaxDays = 7; // Keep the logs for 7 days (-1 = never delete logs)
```

## Use the logger

Use this logger like any other.

Inject in the constructor using dependency injection:

```cs
private readonly ILogger<MyService> _logger;
public MyService(ILogger<MyService> logger)
{
    _logger = logger;
}
```

```cs
_logger.Information("Hello {Name}", "World");
```

## Add extra info the the logs

Use the special `Extra` property of the `ApplicationException` to add extra options.

```cs
var MyException = new ApplicationException("Oops something went wrong", new { myExtraProperty = "MyValue" });
_logger.LogError(MyExeception, "Something went wrong", null);
```

## Use the global handler to log all exceptions

There is a global handler available that is able to log all exceptions coming from the requests.

```cs
app.UseMiddleware<ErrorHandlerMiddleware>();
```
This middleware comes with a default implementation that add headers and claims as an extra info to the logs.

You can change the information logged by implementing your own delegate:

```
Func<HttpContext, object> myImplementation = delegate(HttpContext context)
{
    var myValue = "Hello World"
    return new 
    {
        myProperty = myValue
    };
};

app.UseMiddleware<ErrorHandlerMiddleware>(myImplementation);
```

## Create the table in the database

```sql
CREATE TABLE [dbo].[Logs](
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[LogLevel] [nvarchar](50) NULL,
	[Extra] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[ExceptionType] [nvarchar](max) NULL
)
```

## Join with your tables to extract data

```sql
SELECT TOP(20) * FROM Logs L JOIN Users U ON JSON_VALUE(L.Extra, '$.claims.userId') = U.Id WHERE U.Id = 25
```
