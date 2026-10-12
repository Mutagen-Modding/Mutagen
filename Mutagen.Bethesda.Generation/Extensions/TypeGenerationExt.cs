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

    /// <summary>
    /// What the class holds for the field when the record leaves it out.
    /// </summary>
    public static string UnsetValue(this TypeGeneration typeGen, string fallback)
    {
        return typeGen is TypicalTypeGeneration { HasDefault: true, Nullable: false } typical
            ? $"{typeGen.ObjectGen.ObjectName}.{typical.DefaultValueMemberName}"
            : fallback;
    }

    public static bool NeedsRecordConverter(this TypeGeneration typeGen)
    {
        return typeGen is LoquiType loqui
               && loqui.GetFieldData().HasTrigger;
    }
}