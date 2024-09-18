namespace Mutagen.Bethesda.Plugins.Exceptions;

public class MissingModMappingException : Exception
{
    public MissingModMappingException()
    {
    }

    public MissingModMappingException(string? message) : base(message)
    {
    }

    public MissingModMappingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}