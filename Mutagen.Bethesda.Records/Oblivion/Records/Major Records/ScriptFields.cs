using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Notifying;
using ReactiveUI;
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

        public IEnumerable<ScriptObjectReference> ObjectReferences => this.References.WhereCastable<ScriptReference, ScriptObjectReference>();
        public IEnumerable<ScriptVariableReference> VariableReferences => this.References.WhereCastable<ScriptReference, ScriptVariableReference>();

        #region CompiledScript
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool CompiledScript_IsSet
        {
            get => _hasBeenSetTracker[(int)ScriptFields_FieldIndex.CompiledScript];
            set => _hasBeenSetTracker[(int)ScriptFields_FieldIndex.CompiledScript] = value;
        }
        bool IScriptFieldsGetter.CompiledScript_IsSet => CompiledScript_IsSet;
        protected Byte[] _CompiledScript;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Byte[] CompiledScript
        {
            get => this._CompiledScript;
            set => CompiledScript_Set(value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ReadOnlySpan<byte> IScriptFieldsGetter.CompiledScript => this.CompiledScript;
        public void CompiledScript_Set(
            Byte[] value,
            bool markSet = true)
        {
            this.MetadataSummary.CompiledSizeInternal = value?.Length ?? 0;
            _CompiledScript = value;
            _hasBeenSetTracker[(int)ScriptFields_FieldIndex.CompiledScript] = markSet;
        }
        public void CompiledScript_Unset()
        {
            this.MetadataSummary.CompiledSizeInternal = 0;
            this.CompiledScript_Set(default(Byte[]), false);
        }
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

            static partial void FillBinaryMetadataSummaryOldCustom(MutagenFrame frame, IScriptFields item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                item.MetadataSummary.CopyInFromBinary(
                    frame: frame,
                    masterReferences: masterReferences,
                    recordTypeConverter: metaConverter,
                    errorMask: errorMask);
            }
        }
    }
}
