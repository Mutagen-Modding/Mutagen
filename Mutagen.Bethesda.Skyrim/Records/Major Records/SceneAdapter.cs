using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Skyrim;

partial class SceneAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, ISceneAdapter item)
    {
        item.ScriptFragments = Mutagen.Bethesda.Skyrim.SceneScriptFragments.CreateFromBinary(frame: frame);
    }
}

partial class SceneAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, ISceneAdapterGetter item)
    {
        if (item.ScriptFragments is not {} frags) return;
        frags.WriteToBinary(writer);
    }
}

partial class SceneAdapterBinaryOverlay
{
    public partial ISceneScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
    {
        if (this.ScriptsEndingPos == _data.Length) return null;
        return SceneScriptFragmentsBinaryOverlay.SceneScriptFragmentsFactory(
            _data.Slice(this.ScriptsEndingPos),
            _package);
    }
}