using Noggog;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class ModHeaderMalformedException : Exception
{
    public ModPath? ModPath { get; }

    public ModHeaderMalformedException(ModKey key, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ModPath = new ModPath(key, string.Empty);
    }

    public ModHeaderMalformedException(ModPath path, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ModPath = path;
    }

    public ModHeaderMalformedException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    public override string ToString()
    {
        return $"{GetType().Name}{(ModPath == null ? string.Empty : $" {ModPath}")}: {Message} {InnerException}{StackTrace}";
    }
}