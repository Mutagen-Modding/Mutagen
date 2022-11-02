namespace Mutagen.Bethesda.Plugins.Exceptions;

public class SelfReferenceException : Exception
{
    public ModKey ModKey { get; }

    public SelfReferenceException(ModKey modKey)
        : base($"Cannot add mod to itself as a master: {modKey}")
    {
        ModKey = modKey;
    }
}