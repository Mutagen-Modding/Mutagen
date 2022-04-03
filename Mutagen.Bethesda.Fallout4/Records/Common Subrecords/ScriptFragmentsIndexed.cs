using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Fallout4
{
    namespace Internals
    {
        public partial class ScriptFragmentsIndexedBinaryOverlay
        {
            public sbyte ExtraBindDataVersion => throw new NotImplementedException();

            public IScriptEntryGetter Script => throw new NotImplementedException();

            public IReadOnlyList<IScriptFragmentIndexedGetter> Fragments => throw new NotImplementedException();
        }
    }
}
