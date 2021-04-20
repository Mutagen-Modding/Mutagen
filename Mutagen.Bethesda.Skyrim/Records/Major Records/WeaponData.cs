using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class WeaponData
    {
        [Flags]
        public enum Flag : uint
        {
            IgnoresNormalWeaponResistance = 0x0000_0001,
            Automatic = 0x0000_0002,
            HasScope = 0x0000_0004,
            CantDrop = 0x0000_0008,
            HideBackpack = 0x0000_0010,
            EmbeddedWeapon = 0x0000_0020,
            DontUseFirstPersonIsAnim = 0x0000_0040,
            NonPlayable = 0x0000_0080,
            PlayerOnly = 0x0001_0000,
            NPCsUseAmmo = 0x0002_0000,
            NoJamAfterReload = 0x0004_0000,
            MinorCrime = 0x0010_0000,
            RangeFixed = 0x0020_0000,
            NotUsedInNormalCombat = 0x0040_0000,
            DontUseThirdPersonISAnim = 0x0100_0000,
            BurstShot = 0x0200_0000,
            RumbleAlternate = 0x0400_0000,
            LongBursts = 0x0800_0000,
            NonHostile = 0x1000_0000,
            BoundWeapon = 0x2000_0000,
        }

        public enum AttackAnimationType : byte
        {
            Default = 255,
            AttackLeft = 26,
            AttackRight = 32,
            Attack3 = 38,
            Attack4 = 44,
            Attack5 = 50,
            Attack6 = 56,
            Attack7 = 62,
            Attack8 = 68,
            AttackLoop = 74,
            AttackSpin = 80,
            AttackSpin2 = 86,
            PlaceMine = 97,
            PlaceMine2 = 103,
            AttackThrow = 109,
            AttackThrow2 = 115,
            AttackThrow3 = 121,
            AttackThrow4 = 127,
            AttackThrow5 = 133,
        }

        public enum OnHitType
        {
            NoFormulaBehavior = 0,
            DismemberOnly = 1,
            ExplodeOnly = 2,
            NoDismemberOrExplode = 3,
        }
    }

    namespace Internals
    {
        public partial class WeaponDataBinaryCreateTranslation
        {
            public const uint UpperFlagMask = 0xFFFF_0000;
            public const byte UpperFlagShift = 16;

            static partial void FillBinaryFlagsCustom(MutagenFrame frame, IWeaponData item)
            {
                // Read normally
                item.Flags = (WeaponData.Flag)frame.ReadUInt16();
            }

            static partial void FillBinaryFlags2Custom(MutagenFrame frame, IWeaponData item)
            {
                var flags2 = frame.ReadUInt32();
                flags2 <<= UpperFlagShift;

                // Clean existing flags
                var flags = (uint)item.Flags;
                flags &= ~UpperFlagMask;

                flags |= flags2;
                item.Flags = (WeaponData.Flag)flags;
            }
        }

        public partial class WeaponDataBinaryWriteTranslation
        {
            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IWeaponDataGetter item)
            {
                writer.Write((ushort)item.Flags);
            }

            static partial void WriteBinaryFlags2Custom(MutagenWriter writer, IWeaponDataGetter item)
            {
                var flags = (uint)item.Flags;

                // Clean lower flags
                flags &= WeaponDataBinaryCreateTranslation.UpperFlagMask;

                flags >>= WeaponDataBinaryCreateTranslation.UpperFlagShift;
                writer.Write(flags);
            }
        }

        public partial class WeaponDataBinaryOverlay
        {
            WeaponData.Flag GetFlagsCustom(int location)
            {
                var flags = (uint)BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location, 2));
                location += 0x1C;
                var flags2 = (uint)BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location, 2));
                flags2 <<= WeaponDataBinaryCreateTranslation.UpperFlagShift;
                flags |= flags2;
                return (WeaponData.Flag)flags;
            }

            partial void Flags2CustomParse(OverlayStream stream, int offset)
            {
                stream.Position += 4;
            }
        }
    }
}
