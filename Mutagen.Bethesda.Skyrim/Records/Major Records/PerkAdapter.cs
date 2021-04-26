using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PerkAdapterBinaryCreateTranslation
        {
            static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IPerkAdapter item)
            {
                item.ScriptFragments = Mutagen.Bethesda.Skyrim.PerkScriptFragments.CreateFromBinary(frame: frame);
            }
        }

        public partial class PerkAdapterBinaryWriteTranslation
        {
            static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IPerkAdapterGetter item)
            {
                if (!item.ScriptFragments.TryGet(out var frags)) return;
                frags.WriteToBinary(writer);
            }
        }

        public partial class PerkAdapterBinaryOverlay
        {
            IPerkScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
            {
                if (this.ScriptsEndingPos == _data.Length) return null;
                return PerkScriptFragmentsBinaryOverlay.PerkScriptFragmentsFactory(
                    _data.Slice(this.ScriptsEndingPos),
                    _package);
            }
        }
    }
}
