using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout3;

public partial class ScriptFields
{
    public enum ScriptType : ushort
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


