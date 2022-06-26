namespace Mutagen.Bethesda.Plugins.Exceptions;

public class ModHeaderMalformedException : ModPathException
{
    public ModHeaderMalformedException(ModPath path, string? message = null, Exception? innerException = null) : base(path, message, innerException)
    {
    }

    public ModHeaderMalformedException(IEnumerable<ModPath> path, string? message = null, Exception? innerException = null) : base(path, message, innerException)
    {
    }

    public ModHeaderMalformedException(ModKey key, string? message = null, Exception? innerException = null) : base(key, message, innerException)
    {
    }

    public ModHeaderMalformedException(IEnumerable<ModKey> keys, string? message = null, Exception? innerException = null) : base(keys, message, innerException)
    {
    }
}