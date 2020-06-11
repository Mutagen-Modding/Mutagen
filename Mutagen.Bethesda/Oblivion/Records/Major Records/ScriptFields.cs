using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public IEnumerable<ScriptObjectReference>? ObjectReferences => this.References?.WhereCastable<ScriptReference, ScriptObjectReference>();
        public IEnumerable<ScriptVariableReference>? VariableReferences => this.References?.WhereCastable<ScriptReference, ScriptVariableReference>();

        #region CompiledScript
        protected MemorySlice<byte>? _CompiledScript;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MemorySlice<byte>? CompiledScript
        {
            get => this._CompiledScript;
            set
            {
                this._CompiledScript = value;
                this.MetadataSummary.CompiledSizeInternal = value?.Length ?? 0;
            }
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ReadOnlyMemorySlice<byte>? IScriptFieldsGetter.CompiledScript => this.CompiledScript ?? default(ReadOnlyMemorySlice<byte>?);
        #endregion
    }

    namespace Internals
    {
        public partial class ScriptFieldsBinaryCreateTranslation
        {
            private readonly static RecordTypeConverter metaConverter = new RecordTypeConverter(
                new KeyValuePair<RecordType, RecordType>(
                    new RecordType("SCHR"),
                    new RecordType("SCHD")));

            static partial void FillBinaryMetadataSummaryOldCustom(MutagenFrame frame, IScriptFields item)
            {
                item.MetadataSummary.CopyInFromBinary(
                    frame: frame,
                    recordTypeConverter: metaConverter);
            }
        }
    }
}
