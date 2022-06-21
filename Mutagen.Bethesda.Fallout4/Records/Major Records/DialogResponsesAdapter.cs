using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Fallout4;

partial class DialogResponsesAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IDialogResponsesAdapter item)
    {
        item.ScriptFragments = ScriptFragmentsBinaryCreateTranslation.ReadFragments(frame: frame, objectFormat: item.ObjectFormat);
    }
}

partial class DialogResponsesAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IDialogResponsesAdapterGetter item)
    {
        if (item.ScriptFragments is not { } frags) return;
        ScriptFragmentsBinaryWriteTranslation.WriteFragments(writer, item.ScriptFragments, objectFormat: item.ObjectFormat);
    }
}

partial class DialogResponsesAdapterBinaryOverlay
{
    public partial IScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
    {
        if (this.ScriptsEndingPos == _structData.Length) return null;
        return ScriptFragmentsBinaryCreateTranslation.ReadFragments(frame: new MutagenFrame(
            new MutagenMemoryReadStream(_structData.Slice(ScriptsEndingPos), _package.MetaData)), objectFormat: this.ObjectFormat);
    }
}
