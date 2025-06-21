using System.Diagnostics;
using Mutagen.Bethesda.Plugins;

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
    IVirtualMachineAdapterGetter? IPlacedGetter.VirtualMachineAdapter => this.VirtualMachineAdapter;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<IEmittanceGetter> IPlacedGetter.Emittance => this.Emittance;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<IPlacedObjectGetter> IPlacedGetter.MultiBoundReference => this.MultiBoundReference;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IActivateParentsGetter? IPlacedGetter.ActivateParents => this.ActivateParents;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<IEncounterZoneGetter> IPlacedGetter.EncounterZone => this.EncounterZone;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IFormLinkGetter<ILocationReferenceTypeGetter>>? IPlacedGetter.LocationRefTypes => this.LocationRefTypes;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<IOwnerGetter> IPlacedGetter.Owner => this.Owner;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<ILocationGetter> IPlacedGetter.LocationReference => this.LocationReference;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnableParentGetter? IPlacedGetter.EnableParent => this.EnableParent;
}