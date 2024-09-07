namespace Mutagen.Bethesda.Plugins.Exceptions;

public class MissingLoadOrderException : Exception
{
    public MissingLoadOrderException()
    {
    }

    public MissingLoadOrderException(string? message) : base(message)
    {
    }

    public MissingLoadOrderException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}