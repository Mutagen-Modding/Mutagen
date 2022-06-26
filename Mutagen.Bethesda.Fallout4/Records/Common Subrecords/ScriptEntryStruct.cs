using Mutagen.Bethesda.Plugins.Aspects;
using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout4;

public partial class ScriptEntryStruct : ScriptEntry
{
}

partial class ScriptEntryStructBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string INamedRequiredGetter.Name => this.Name ?? string.Empty;

    public IReadOnlyList<IScriptPropertyStructGetter> Properties => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public ScriptEntry.Flag Flags => throw new NotImplementedException();
}