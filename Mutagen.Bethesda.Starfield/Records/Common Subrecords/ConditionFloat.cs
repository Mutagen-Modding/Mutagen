using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

partial class ConditionFloatBinaryOverlay
{
    public float ComparisonValue => _structData.Slice(4).Float();
}

partial class ConditionFloatBinaryCreateTranslation
{
    public static partial void CustomBinaryEndImport(
        MutagenFrame frame,
        IConditionFloat obj)
    {
        ConditionBinaryCreateTranslation.CustomStringImports(frame.Reader, obj.Data);
    }
}

partial class ConditionFloatBinaryWriteTranslation
{
    public static partial void CustomBinaryEndExport(
        MutagenWriter writer,
        IConditionFloatGetter obj)
    {
        CustomStringExports(writer, obj.Data);
    }
}