namespace Mutagen.Bethesda.Plugins.Exceptions;

public class MalformedDataException : System.Exception
{
    public MalformedDataException(string? message)
        : base(message)
    {
    }
}
