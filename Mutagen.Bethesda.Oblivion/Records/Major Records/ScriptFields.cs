using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class ScriptFields
    {
        public enum ScriptType
        {
            Object = 0,
            Quest = 1,
            MagicEffect = 0x100
        }

        private readonly static RecordTypeConverter metaConverter = new RecordTypeConverter(
            new KeyValuePair<RecordType, RecordType>(
                new RecordType("SCHR"),
                new RecordType("SCHD")));
        
        partial void CustomCtor()
        {
            this.CompiledScript_Property.Subscribe(
                (change) =>
                {
                    this.MetadataSummary.CompiledSizeInternal = change.New?.Length ?? 0;
                },
                cmds: NotifyingSubscribeParameters.NoFire);
        }

        static partial void FillBinary_MetadataSummaryOld_Custom(MutagenFrame frame, ScriptFields item, ErrorMaskBuilder errorMask)
        {
            var tmpMetadataSummary = ScriptMetaSummary.Create_Binary(
                frame: frame,
                errorMask: errorMask,
                recordTypeConverter: metaConverter);
            item.MetadataSummary.CopyFieldsFrom(
                rhs: tmpMetadataSummary,
                def: null,
                cmds: null,
                copyMask: null,
                errorMask: errorMask);
        }

        static partial void WriteBinary_MetadataSummaryOld_Custom(MutagenWriter writer, ScriptFields item, ErrorMaskBuilder errorMask)
        {
        }
    }
}
