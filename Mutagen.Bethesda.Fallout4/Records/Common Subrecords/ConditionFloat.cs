using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class ConditionFloat
    {
        #region Data
        public ConditionData Data { get; set; } = new ConditionData();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConditionDataGetter IConditionFloatGetter.Data => Data;
        #endregion
    }
}
