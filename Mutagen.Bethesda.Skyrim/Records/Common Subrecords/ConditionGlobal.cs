using System.Diagnostics;

namespace Mutagen.Bethesda.Skyrim;

public partial class ConditionGlobal
{
    #region Data
    public override ConditionData Data { get; set; } = new GetWantBlockingConditionData();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IConditionDataGetter IConditionGlobalGetter.Data => Data;
    #endregion
}

partial class ConditionGlobalBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IConditionDataGetter IConditionGlobalGetter.Data => Data;
}