using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog.Notifying;
using ReactiveUI;
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

        public IEnumerable<ScriptObjectReference> ObjectReferences => this.References.Items.WhereCastable<ScriptReference, ScriptObjectReference>();
        public IEnumerable<ScriptVariableReference> VariableReferences => this.References.Items.WhereCastable<ScriptReference, ScriptVariableReference>();

        private readonly static RecordTypeConverter metaConverter = new RecordTypeConverter(
            new KeyValuePair<RecordType, RecordType>(
                new RecordType("SCHR"),
                new RecordType("SCHD")));
        
        partial void CustomCtor()
        {
            this.WhenAny(x => x.CompiledScript).Subscribe((change) =>
            {
                this.MetadataSummary.CompiledSizeInternal = change?.Length ?? 0;
            });
        }

        static partial void FillBinary_MetadataSummaryOld_Custom(MutagenFrame frame, ScriptFields item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            var tmpMetadataSummary = ScriptMetaSummary.Create_Binary(
                frame: frame,
                errorMask: errorMask,
                masterReferences: masterReferences,
                recordTypeConverter: metaConverter);
            item.MetadataSummary.CopyFieldsFrom(
                rhs: tmpMetadataSummary,
                def: null,
                copyMask: null,
                errorMask: errorMask);
        }

        static partial void WriteBinary_MetadataSummaryOld_Custom(MutagenWriter writer, ScriptFields item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
        }
    }
}
