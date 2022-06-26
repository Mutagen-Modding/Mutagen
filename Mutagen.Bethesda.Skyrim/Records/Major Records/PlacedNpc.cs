using System.Diagnostics;

namespace Mutagen.Bethesda.Skyrim;

public partial class PlacedNpc
{
    [Flags]
    public enum MajorFlag : uint
    {
        StartsDead = 0x0000_0200,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        NoAiAcquire = 0x0200_0000,
        DoNotHavokSettle = 0x2000_0000,
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IPlacementGetter? IPlacedGetter.Placement => this.Placement;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnableParentGetter? IPlacedGetter.EnableParent => this.EnableParent;
}