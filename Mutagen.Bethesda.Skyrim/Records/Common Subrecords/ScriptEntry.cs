using Mutagen.Bethesda.Plugins.Aspects;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mutagen.Bethesda.Skyrim;

public partial class ScriptEntry
{
    public enum Flag : byte
    {
        Local = 0,
        Inherited = 1,
        Removed = 2,
        InheritedAndRemoved = 3,
    }
}

partial class ScriptEntryBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string INamedRequiredGetter.Name => this.Name ?? string.Empty;

    public IReadOnlyList<IScriptPropertyGetter> Properties => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public ScriptEntry.Flag Flags => throw new NotImplementedException();
}