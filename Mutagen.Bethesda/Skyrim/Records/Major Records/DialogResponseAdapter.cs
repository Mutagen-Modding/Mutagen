using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class DialogResponseAdapterBinaryCreateTranslation
        {
            static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IDialogResponseAdapter item)
            {
                item.ScriptFragments = Mutagen.Bethesda.Skyrim.ScriptFragments.CreateFromBinary(frame: frame);
            }
        }

        public partial class DialogResponseAdapterBinaryWriteTranslation
        {
            static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IDialogResponseAdapterGetter item)
            {
                if (!item.ScriptFragments.TryGet(out var frags)) return;
                frags.WriteToBinary(writer);
            }
        }

        public partial class DialogResponseAdapterBinaryOverlay
        {
            IScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
            {
                if (this.ScriptsEndingPos == _data.Length) return null;
                return ScriptFragmentsBinaryOverlay.ScriptFragmentsFactory(
                    _data.Slice(this.ScriptsEndingPos),
                    _package);
            }
        }
    }
}
