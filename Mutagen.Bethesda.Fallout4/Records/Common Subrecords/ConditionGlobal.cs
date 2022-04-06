using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout4;

public partial class ConditionGlobal
{
    #region Data
    public override ConditionData Data { get; set; } = new FunctionConditionData();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IConditionDataGetter IConditionGlobalGetter.Data => Data;
    #endregion
}

partial class ConditionGlobalBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IConditionDataGetter IConditionGlobalGetter.Data => Data;
}