using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class DialogResponsesAdapterBinaryCreateTranslation
        {
            public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, IDialogResponsesAdapter item)
            {
                item.ScriptFragments = Mutagen.Bethesda.Skyrim.ScriptFragments.CreateFromBinary(frame: frame);
            }
        }

        public partial class DialogResponsesAdapterBinaryWriteTranslation
        {
            public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, IDialogResponsesAdapterGetter item)
            {
                if (item.ScriptFragments is not { } frags) return;
                frags.WriteToBinary(writer);
            }
        }

        public partial class DialogResponsesAdapterBinaryOverlay
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
