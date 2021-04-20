using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Overlay;
using System;
using System.Diagnostics;

namespace Mutagen.Bethesda.Skyrim
{
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
        IEnableParentGetter? IPlacedGetter.EnableParent => this.EnableParent;
    }

    namespace Internals
    {
        public partial class APlacedTrapBinaryCreateTranslation
        {
            static partial void FillBinaryTrapFormCustom(MutagenFrame frame, IAPlacedTrapInternal item)
            {
                var subRec = frame.ReadSubrecordFrame();
                if (subRec.Content.Length != 4)
                {
                    throw new ArgumentException("Unexpected length");
                }
                var form = FormKeyBinaryTranslation.Instance.Parse(subRec.Content, frame.MetaData.MasterReferences!);
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
            }
        }

        public partial class APlacedTrapBinaryWriteTranslation
        {
            static partial void WriteBinaryTrapFormCustom(MutagenWriter writer, IAPlacedTrapGetter item)
            {
                using var header = HeaderExport.Subrecord(writer, RecordTypes.NAME);
                switch (item)
                {
                    case IPlacedArrowGetter arrow:
                        FormKeyBinaryTranslation.Instance.Write(writer, arrow.Projectile.FormKey);
                        break;
                    case IPlacedBeamGetter beam:
                        FormKeyBinaryTranslation.Instance.Write(writer, beam.Projectile.FormKey);
                        break;
                    case IPlacedFlameGetter flame:
                        FormKeyBinaryTranslation.Instance.Write(writer, flame.Projectile.FormKey);
                        break;
                    case IPlacedConeGetter cone:
                        FormKeyBinaryTranslation.Instance.Write(writer, cone.Projectile.FormKey);
                        break;
                    case IPlacedBarrierGetter barrier:
                        FormKeyBinaryTranslation.Instance.Write(writer, barrier.Projectile.FormKey);
                        break;
                    case IPlacedTrapGetter trap:
                        FormKeyBinaryTranslation.Instance.Write(writer, trap.Projectile.FormKey);
                        break;
                    case IPlacedHazardGetter hazard:
                        FormKeyBinaryTranslation.Instance.Write(writer, hazard.Hazard.FormKey);
                        break;
                    case IPlacedMissileGetter missile:
                        FormKeyBinaryTranslation.Instance.Write(writer, missile.Projectile.FormKey);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public partial class APlacedTrapBinaryOverlay
        {
            partial void TrapFormCustomParse(OverlayStream stream, int offset)
            {
                var subRec = stream.ReadSubrecordFrame();
                if (subRec.Content.Length != 4)
                {
                    throw new ArgumentException("Unexpected length");
                }
                var form = FormKeyBinaryTranslation.Instance.Parse(subRec.Content, _package.MetaData.MasterReferences!);
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
            }
        }
    }
}
