using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class DialogResponsesAdapterBinaryCreateTranslation
        {
            static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IDialogResponsesAdapter item)
            {
                item.ScriptFragments = Mutagen.Bethesda.Skyrim.DialogResponsesScriptFragments.CreateFromBinary(frame: frame);
            }
        }

        public partial class DialogResponsesAdapterBinaryWriteTranslation
        {
            static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IDialogResponsesAdapterGetter item)
            {
                if (!item.ScriptFragments.TryGet(out var frags)) return;
                frags.WriteToBinary(writer);
            }
        }

        public partial class DialogResponsesAdapterBinaryOverlay
        {
            IDialogResponsesScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
            {
                if (this.ScriptsEndingPos == _data.Length) return null;
                return DialogResponsesScriptFragmentsBinaryOverlay.DialogResponsesScriptFragmentsFactory(
                    _data.Slice(this.ScriptsEndingPos),
                    _package);
            }
        }
    }
}
