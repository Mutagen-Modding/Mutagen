using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ConditionFloat
    {
        #region Data
        public override ConditionData Data { get; set; } = new FunctionConditionData();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConditionDataGetter IConditionFloatGetter.Data => Data;
        #endregion
    }

    namespace Internals
    {
        public partial class ConditionFloatBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IConditionDataGetter IConditionFloatGetter.Data => Data;
        }
    }
}
