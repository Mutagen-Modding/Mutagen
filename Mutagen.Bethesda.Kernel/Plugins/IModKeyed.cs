namespace Mutagen.Bethesda.Plugins;

public interface IModKeyed
{
    /// <summary>
    /// The associated ModKey
    /// </summary>
    ModKey ModKey { get; }
}