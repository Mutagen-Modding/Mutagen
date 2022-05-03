namespace Mutagen.Bethesda.Plugins.Exceptions;

public class MalformedDataException : Exception
{
    public MalformedDataException(string? message)
        : base(message)
    {
    }
}
