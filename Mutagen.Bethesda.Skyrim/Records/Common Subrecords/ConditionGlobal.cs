using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim;

internal partial class ConditionGlobalBinaryOverlay
{
    public IFormLinkGetter<IGlobalGetter> ComparisonValue => FormLinkBinaryTranslation.Instance.NullableOverlayFactory<IGlobalGetter>(_package, _structData.Slice(4));
}

partial class ConditionGlobalBinaryCreateTranslation
{
    public static partial void CustomBinaryEndImport(
        MutagenFrame frame,
        IConditionGlobal obj)
    {
        ConditionBinaryCreateTranslation.CustomStringImports(frame.Reader, obj.Data);
    }
}

partial class ConditionGlobalBinaryWriteTranslation
{
    public static partial void CustomBinaryEndExport(
        MutagenWriter writer,
        IConditionGlobalGetter obj)
    {
        CustomStringExports(writer, obj.Data);
    }
}