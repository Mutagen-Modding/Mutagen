using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PackageAdapterBinaryCreateTranslation
        {
            public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IPackageAdapter item)
            {
                item.ScriptFragments = Mutagen.Bethesda.Skyrim.PackageScriptFragments.CreateFromBinary(frame: frame);
            }
        }

        public partial class PackageAdapterBinaryWriteTranslation
        {
            public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IPackageAdapterGetter item)
            {
                if (item.ScriptFragments is not {} frags) return;
                frags.WriteToBinary(writer);
            }
        }

        public partial class PackageAdapterBinaryOverlay
        {
            public partial IPackageScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
            {
                if (this.ScriptsEndingPos == _data.Length) return null;
                return PackageScriptFragmentsBinaryOverlay.PackageScriptFragmentsFactory(
                    _data.Slice(this.ScriptsEndingPos),
                    _package);
            }
        }
    }
}
