using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Fallout4;

partial class PerkAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IPerkAdapter item)
    {
        item.ScriptFragments = PerkScriptFragmentsBinaryCreateTranslation.ReadFragments(frame: frame, objectFormat: item.ObjectFormat);
    }
}

partial class PerkAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IPerkAdapterGetter item)
    {
        if (item.ScriptFragments is not { } frags) return;
        PerkScriptFragmentsBinaryWriteTranslation.WriteFragments(writer, frags, item.ObjectFormat);
    }
}

partial class PerkAdapterBinaryOverlay
{
    public partial IPerkScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
    {
        if (this.ScriptsEndingPos == _data.Length) return null;
        return PerkScriptFragmentsBinaryCreateTranslation.ReadFragments(frame: new MutagenFrame(
            new OverlayStream(_data.Slice(ScriptsEndingPos), _package)), objectFormat: ObjectFormat);
    }
}