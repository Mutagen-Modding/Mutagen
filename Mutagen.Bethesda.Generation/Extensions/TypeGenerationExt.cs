using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Noggog;

namespace Mutagen.Bethesda.Generation;

public static class TypeGenerationExt
{
    public static MutagenFieldData GetFieldData(this TypeGeneration typeGen)
    {
        return typeGen.CustomData.GetOrAdd(Mutagen.Bethesda.Generation.Constants.DataKey, () => new MutagenFieldData(typeGen)) as MutagenFieldData;
    }

    public static bool NeedsRecordConverter(this TypeGeneration typeGen)
    {
        return typeGen is LoquiType loqui
               && loqui.GetFieldData().HasTrigger;
    }
}