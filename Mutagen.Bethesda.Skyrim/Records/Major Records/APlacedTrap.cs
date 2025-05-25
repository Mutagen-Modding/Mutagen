using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System.Diagnostics;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class APlacedTrap
{
    [Flags]
    public enum MajorFlag : uint
    {
        TurnOffFire = 0x0000_0080,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        ReflectedByAutoWater = 0x1000_0000,
        DontHavokSettle = 0x2000_0000,
        NoRespawn = 0x4000_0000,
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

partial class APlacedTrapBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryTrapFormCustom(MutagenFrame frame, IAPlacedTrapInternal item, PreviousParse lastParsed)
    {
        var subRec = frame.ReadSubrecord();
        if (subRec.Content.Length != 4)
        {
            throw new ArgumentException("Unexpected length");
        }
        var form = FormKeyBinaryTranslation.Instance.Parse(subRec.Content, frame.MetaData.MasterReferences);
        switch (item)
        {
            case IPlacedArrow arrow:
                arrow.Projectile.FormKey = form;
                break;
            case IPlacedBeam beam:
                beam.Projectile.FormKey = form;
                break;
            case IPlacedFlame flame:
                flame.Projectile.FormKey = form;
                break;
            case IPlacedCone cone:
                cone.Projectile.FormKey = form;
                break;
            case IPlacedBarrier barrier:
                barrier.Projectile.FormKey = form;
                break;
            case IPlacedTrap trap:
                trap.Projectile.FormKey = form;
                break;
            case IPlacedHazard hazard:
                hazard.Hazard.FormKey = form;
                break;
            case IPlacedMissile missile:
                missile.Projectile.FormKey = form;
                break;
            default:
                throw new NotImplementedException();
        }

        return null;
    }
}

partial class APlacedTrapBinaryWriteTranslation
{
    public static partial void WriteBinaryTrapFormCustom(MutagenWriter writer, IAPlacedTrapGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.NAME);
        switch (item)
        {
            case IPlacedArrowGetter arrow:
                FormKeyBinaryTranslation.Instance.Write(writer, arrow.Projectile);
                break;
            case IPlacedBeamGetter beam:
                FormKeyBinaryTranslation.Instance.Write(writer, beam.Projectile);
                break;
            case IPlacedFlameGetter flame:
                FormKeyBinaryTranslation.Instance.Write(writer, flame.Projectile);
                break;
            case IPlacedConeGetter cone:
                FormKeyBinaryTranslation.Instance.Write(writer, cone.Projectile);
                break;
            case IPlacedBarrierGetter barrier:
                FormKeyBinaryTranslation.Instance.Write(writer, barrier.Projectile);
                break;
            case IPlacedTrapGetter trap:
                FormKeyBinaryTranslation.Instance.Write(writer, trap.Projectile);
                break;
            case IPlacedHazardGetter hazard:
                FormKeyBinaryTranslation.Instance.Write(writer, hazard.Hazard);
                break;
            case IPlacedMissileGetter missile:
                FormKeyBinaryTranslation.Instance.Write(writer, missile.Projectile);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class APlacedTrapBinaryOverlay
{
    public partial ParseResult TrapFormCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        var subRec = stream.ReadSubrecord();
        if (subRec.Content.Length != 4)
        {
            throw new ArgumentException("Unexpected length");
        }
        var form = FormKeyBinaryTranslation.Instance.Parse(subRec.Content, _package.MetaData.MasterReferences);
        switch (this)
        {
            case PlacedArrowBinaryOverlay arrow:
                arrow.Projectile = new FormLink<IProjectileGetter>(form);
                break;
            case PlacedBeamBinaryOverlay beam:
                beam.Projectile = new FormLink<IProjectileGetter>(form);
                break;
            case PlacedFlameBinaryOverlay flame:
                flame.Projectile = new FormLink<IProjectileGetter>(form);
                break;
            case PlacedConeBinaryOverlay cone:
                cone.Projectile = new FormLink<IProjectileGetter>(form);
                break;
            case PlacedBarrierBinaryOverlay barrier:
                barrier.Projectile = new FormLink<IProjectileGetter>(form);
                break;
            case PlacedTrapBinaryOverlay trap:
                trap.Projectile = new FormLink<IProjectileGetter>(form);
                break;
            case PlacedHazardBinaryOverlay hazard:
                hazard.Hazard = new FormLink<IHazardGetter>(form);
                break;
            case PlacedMissileBinaryOverlay missile:
                missile.Projectile = new FormLink<IProjectileGetter>(form);
                break;
            default:
                throw new NotImplementedException();
        }

        return null;
    }
}