using System.Diagnostics;

namespace Mutagen.Bethesda.Skyrim;

public partial class ConditionFloat
{
    #region Data
    public override ConditionData Data { get; set; } = new GetWantBlockingConditionData();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IConditionDataGetter IConditionFloatGetter.Data => Data;
    #endregion
}

partial class ConditionFloatBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IConditionDataGetter IConditionFloatGetter.Data => Data;
}