using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class TooManyMastersException : Exception
{
    public ModKey CurrentMod { get; }
    
    private readonly ModKey[] _masters;

    public IReadOnlyList<ModKey> Masters => _masters;

    public TooManyMastersException(
        ModKey currentMod,
        ModKey[] masters)
        : base($"{currentMod} has too many masters on masters list. {masters.Length} >= {Constants.PluginMasterLimit}.")
    {
        CurrentMod = currentMod;
        _masters = masters;
    }
}