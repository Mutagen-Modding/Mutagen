using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Fallout4;

partial class SceneAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryScriptFragmentsCustom(
        MutagenFrame frame,
        ISceneAdapter item)
    {
        item.ScriptFragments = SceneScriptFragmentsBinaryCreateTranslation.ReadFragments(frame: frame, objectFormat: item.ObjectFormat);
    }
}

partial class SceneAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, ISceneAdapterGetter item)
    {
        if (item.ScriptFragments is not { } frags) return;
        SceneScriptFragmentsBinaryWriteTranslation.WriteFragments(writer: writer, objectFormat: item.ObjectFormat, item: item.ScriptFragments);
    }
}

partial class SceneAdapterBinaryOverlay
{
    public partial ISceneScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
    {
        if (this.ScriptsEndingPos == _structData.Length) return null;
        return SceneScriptFragmentsBinaryCreateTranslation.ReadFragments(frame: new MutagenFrame(
            new OverlayStream(_structData.Slice(ScriptsEndingPos), _package)), objectFormat: ObjectFormat);
    }
}
