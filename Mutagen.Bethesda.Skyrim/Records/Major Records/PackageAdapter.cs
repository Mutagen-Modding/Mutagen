using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Skyrim;

partial class PackageAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IPackageAdapter item)
    {
        item.ScriptFragments = Mutagen.Bethesda.Skyrim.PackageScriptFragments.CreateFromBinary(frame: frame);
    }
}

partial class PackageAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IPackageAdapterGetter item)
    {
        if (item.ScriptFragments is not {} frags) return;
        frags.WriteToBinary(writer);
    }
}

partial class PackageAdapterBinaryOverlay
{
    public partial IPackageScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
    {
        if (ScriptsEndingPos == _structData.Length) return null;
        return PackageScriptFragmentsBinaryOverlay.PackageScriptFragmentsFactory(
            _structData.Slice(ScriptsEndingPos),
            _package);
    }
}
