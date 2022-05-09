namespace DbLogger.Logger;

public class ApplicationException : Exception
{
    public ApplicationException(object extra)
    {
        Extra = extra;
    }

    public ApplicationException(string message, object extra)
        : base(message)
    {
        Extra = extra;
    }

    public ApplicationException(string message, Exception inner, object extra)
        : base(message, inner)
    {
        Extra = extra;
    }
    public object Extra { get; set; }
}
