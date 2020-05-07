using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{ 
    public partial class RaceData
    {
        [Flags]
        public enum Flag : ulong
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
            UseAdvancedAvoidance = 0x0001_0000_0000,
            NonHostile = 0x0002_0000_0000,
            AllowMountedCombat = 0x0010_0000_0000,
        }

        // ToDo
        // Remove HeadPartList enums from flags.  Only one can be active, so should set up a way to force that
    }

    namespace Internals
    {
        public partial class RaceDataBinaryCreateTranslation
        {
            static partial void FillBinaryFlags2Custom(MutagenFrame frame, IRaceData item)
            {
                // Clear out upper flags
                item.Flags &= ((RaceData.Flag)0x00000000FFFFFFFF);

                // Set upper flags
                ulong flags2 = frame.ReadUInt32();
                flags2 <<= 32;
                item.Flags |= ((RaceData.Flag)flags2);
            }
        }

        public partial class RaceDataBinaryWriteTranslation
        {
            static partial void WriteBinaryFlags2Custom(MutagenWriter writer, IRaceDataGetter item)
            {
                ulong flags = (ulong)item.Flags;
                flags >>= 32;
                writer.Write((uint)flags);
            }
        }

        public partial class RaceDataBinaryOverlay
        {
            public RaceData.Flag GetFlagsCustom(int location)
            {
                var flag = (RaceData.Flag)BinaryPrimitives.ReadInt32LittleEndian(_data.Span.Slice(location, 4));

                // Clear out upper flags
                flag &= ((RaceData.Flag)0x00000000FFFFFFFF);

                // Set upper flags
                ulong flags2 = BinaryPrimitives.ReadUInt32LittleEndian(_data.Span.Slice(124, 4));
                flags2 <<= 32;
                flag |= ((RaceData.Flag)flags2);
                return flag;
            }
        }
    }
}
