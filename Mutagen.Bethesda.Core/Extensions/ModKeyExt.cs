using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda;

public static class ModKeyExt
{
    public static FormKey MakeFormKey(this ModKey modKey, uint id)
    {
        return new FormKey(modKey, id);
    }
}