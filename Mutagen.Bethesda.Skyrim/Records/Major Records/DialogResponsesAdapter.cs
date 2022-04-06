using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Skyrim;

partial class DialogResponsesAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IDialogResponsesAdapter item)
    {
        item.ScriptFragments = Mutagen.Bethesda.Skyrim.ScriptFragments.CreateFromBinary(frame: frame);
    }
}

partial class DialogResponsesAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IDialogResponsesAdapterGetter item)
    {
        if (item.ScriptFragments is not { } frags) return;
        frags.WriteToBinary(writer);
    }
}

partial class DialogResponsesAdapterBinaryOverlay
{
    public partial IScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
    {
        if (this.ScriptsEndingPos == _data.Length) return null;
        return ScriptFragmentsBinaryOverlay.ScriptFragmentsFactory(
            _data.Slice(this.ScriptsEndingPos),
            _package);
    }
}