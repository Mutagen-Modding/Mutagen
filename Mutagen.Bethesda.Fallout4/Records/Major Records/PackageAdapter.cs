using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Fallout4;

partial class PackageAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IPackageAdapter item)
    {
        item.ScriptFragments = PackageScriptFragmentsBinaryCreateTranslation.ReadFragments(frame: frame, objectFormat: item.ObjectFormat);
    }
}

partial class PackageAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IPackageAdapterGetter item)
    {
        if (item.ScriptFragments is not { } frags) return;
        PackageScriptFragmentsBinaryWriteTranslation.WriteFragments(writer, frags, item.ObjectFormat);
    }
}

partial class PackageAdapterBinaryOverlay
{
    public partial IPackageScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
    {
        if (this.ScriptsEndingPos == _data.Length) return null;
        return PackageScriptFragmentsBinaryCreateTranslation.ReadFragments(frame: new MutagenFrame(
            new OverlayStream(_data.Slice(ScriptsEndingPos), _package)), objectFormat: ObjectFormat);
    }
}
