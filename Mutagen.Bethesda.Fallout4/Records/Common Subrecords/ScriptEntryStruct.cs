using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins.Aspects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class ScriptEntryStruct : ScriptEntry
    {

    }

    namespace Internals
    {
        public partial class ScriptEntryStructBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name ?? string.Empty;

            public IReadOnlyList<IScriptPropertyStructGetter> Properties => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public ScriptEntryStruct.Flag Flags => throw new NotImplementedException();
        }
    }
}
