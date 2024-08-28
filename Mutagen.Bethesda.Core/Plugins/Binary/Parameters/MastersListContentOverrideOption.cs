namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

public class MastersListContentOverrideOption : AMastersListContentOption
{
    public IReadOnlyCollection<ModKey> ModKeys { get; }

    public MastersListContentOverrideOption(IReadOnlyCollection<ModKey> modKeys)
    {
        ModKeys = modKeys;
    }
}