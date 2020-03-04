using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Race
    {
        internal static readonly RecordType NAM2 = new RecordType("NAM2");
        public bool ExportingExtraNam2 { get; set; }

        [Flags]
        public enum Flag : uint
        {
            Playable = 0x0000_0001,
            FaceGenHead = 0x0000_0002,
            Child = 0x0000_0004,
            TiltFrontBack = 0x0000_0008,
            TiltLeftRight = 0x0000_0010,
            NoShadow = 0x0000_0020,
            Swims = 0x0000_0040,
            Flies = 0x0000_0080,
            Walks = 0x0000_0100,
            Immobile = 0x0000_0200,
            NotPunishable = 0x0000_0400,
            NoCombatInWater = 0x0000_0800,
            NoRotatingToHeadTrack = 0x0000_1000,
            DontShowBloodSpray = 0x0000_2000,
            DontShowBloodDecal = 0x0000_4000,
            UsesHeadTrackAnims = 0x0000_8000,
            SpellsAlignWithMagicNode = 0x0001_0000,
            UseWorldRaycastsForFootIK = 0x0002_0000,
            AllowRagdollCollision = 0x0004_0000,
            RegenHpInCombat = 0x0008_0000,
            CantOpenDoors = 0x0010_0000,
            AllowPcDialog = 0x0020_0000,
            NoKnockdowns = 0x0040_0000,
            AllowPickpocket = 0x0080_0000,
            AlwaysUseProxyController = 0x0100_0000,
            DontShowWeaponBlood = 0x0200_0000,
            OverlayHeadPartList = 0x0400_0000,
            OverrideHeadPartList = 0x0800_0000,
            CanPickupItems = 0x1000_0000,
            AllowMultipleMembraneShaders = 0x2000_0000,
            CanDualWield = 0x4000_0000,
            AvoidsRoads = 0x8000_0000,
        }

        [Flags]
        public enum Flag2
        {
            UseAdvancedAvoidance = 0x01,
            NonHostile = 0x02,
            AllowMountedCombat = 0x10
        }

        public enum Size
        {
            Small,
            Medium,
            Large,
            ExtraLarge
        }
    }

    public partial interface IRace
    {
        new bool ExportingExtraNam2 { get; set; }
    }

    public partial interface IRaceGetter
    {
        bool ExportingExtraNam2 { get; }
    }

    namespace Internals
    {
        public partial class RaceBinaryCreateTranslation
        {
            static partial void FillBinaryExtraMarkersCustom(MutagenFrame frame, IRaceInternal item, MasterReferences masterReferences)
            {
                if (frame.Complete) return;
                var subHeader = frame.MetaData.GetSubRecord(frame);
                if (subHeader.RecordType == Race.NAM2)
                {
                    item.ExportingExtraNam2 = true;
                    frame.Position += subHeader.TotalLength;
                }
            }
        }

        public partial class RaceBinaryWriteTranslation
        {
            static partial void WriteBinaryExtraMarkersCustom(MutagenWriter writer, IRaceGetter item, MasterReferences masterReferences)
            {
                if (item.ExportingExtraNam2)
                {
                    using var header = HeaderExport.ExportSubRecordHeader(writer, Race.NAM2);
                }
            }
        }

        public partial class RaceBinaryOverlay
        {
            public bool ExportingExtraNam2 => throw new NotImplementedException();
            public bool ExportingExtraNam3 => throw new NotImplementedException();
        }

        public partial class RaceDataBinaryCreateTranslation
        {
            static partial void FillBinaryMountDataCustom(MutagenFrame frame, IRaceData item, MasterReferences masterReferences)
            {
                throw new NotImplementedException();
            }
        }

        public partial class RaceDataBinaryWriteTranslation
        {
            static partial void WriteBinaryMountDataCustom(MutagenWriter writer, IRaceDataGetter item, MasterReferences masterReferences)
            {
                throw new NotImplementedException();
            }
        }
    }
}
