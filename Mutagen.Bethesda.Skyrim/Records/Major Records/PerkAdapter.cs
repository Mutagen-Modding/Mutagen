using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Skyrim;

partial class PerkAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IPerkAdapter item)
    {
        item.ScriptFragments = Mutagen.Bethesda.Skyrim.PerkScriptFragments.CreateFromBinary(frame: frame);
    }
}

partial class PerkAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IPerkAdapterGetter item)
    {
        if (item.ScriptFragments is not {} frags) return;
        frags.WriteToBinary(writer);
    }
}

partial class PerkAdapterBinaryOverlay
{
    public partial IPerkScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
    {
        if (ScriptsEndingPos == _structData.Length) return null;
        return PerkScriptFragmentsBinaryOverlay.PerkScriptFragmentsFactory(
            _structData.Slice(this.ScriptsEndingPos),
            _package);
    }
}