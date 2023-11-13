using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

public partial class Flora
{
    public enum Property
    {
        LayeredMaterialSwap = RecordTypeInts.FLMS,
        Keyword = RecordTypeInts.FKEY,
    }

    public const string ObjectModificationName = "TESFlora_InstanceData";
}