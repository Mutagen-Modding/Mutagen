using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using System.Collections.Generic;
using System.Diagnostics;

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

        public IEnumerable<ScriptObjectReference>? ObjectReferences => this.References?.WhereCastable<AScriptReference, ScriptObjectReference>();
        public IEnumerable<ScriptVariableReference>? VariableReferences => this.References?.WhereCastable<AScriptReference, ScriptVariableReference>();

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
        public partial class ScriptFieldsBinaryWriteTranslation
        {
            public static partial void WriteBinaryMetadataSummaryOldCustom(MutagenWriter writer, IScriptFieldsGetter item)
            {
            }
        }

        public partial class ScriptFieldsBinaryCreateTranslation
        {
            private readonly static RecordTypeConverter metaConverter = new RecordTypeConverter(
                new KeyValuePair<RecordType, RecordType>(
                    new RecordType("SCHR"),
                    new RecordType("SCHD")));

            public static partial void FillBinaryMetadataSummaryOldCustom(MutagenFrame frame, IScriptFields item)
            {
                item.MetadataSummary.CopyInFromBinary(
                    frame: frame,
                    recordTypeConverter: metaConverter);
            }
        }
    }
}
