using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ConditionGlobal
    {
        #region Data
        public override ConditionData Data { get; set; } = new FunctionConditionData();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConditionDataGetter IConditionGlobalGetter.Data => Data;
        #endregion
    }

    namespace Internals
    {
        public partial class ConditionGlobalBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IConditionDataGetter IConditionGlobalGetter.Data => Data;
        }
    }
}
