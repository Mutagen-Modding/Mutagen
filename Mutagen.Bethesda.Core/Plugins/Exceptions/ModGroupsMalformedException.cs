using Noggog;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class ModGroupsMalformedException : Exception
{
    public ModPath? ModPath { get; }

    public ModGroupsMalformedException(ModKey key, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ModPath = new ModPath(key, string.Empty);
    }

    public ModGroupsMalformedException(ModPath path, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ModPath = path;
    }

    public ModGroupsMalformedException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    public override string ToString()
    {
        return $"{GetType().Name}{(ModPath == null ? string.Empty : $" {ModPath}")}: {Message} {InnerException}{StackTrace}";
    }

    public static ModGroupsMalformedException Enrich(Exception ex, ModKey modKey)
    {
        return new ModGroupsMalformedException(modKey, innerException: ex);
    }
}