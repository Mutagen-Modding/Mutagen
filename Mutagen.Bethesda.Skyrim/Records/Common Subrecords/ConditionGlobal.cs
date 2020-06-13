using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ConditionGlobal
    {
        #region Data
        public ConditionData Data { get; set; } = new FunctionConditionData();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConditionDataGetter IConditionGlobalGetter.Data => Data;
        #endregion
    }
}
