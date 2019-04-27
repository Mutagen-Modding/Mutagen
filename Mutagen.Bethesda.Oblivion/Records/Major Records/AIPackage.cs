using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class AIPackage
    {
        [Flags]
        public enum Flag
        {
            OffersServices = 0x0000001,
            MustReachLocation = 0x0000002,
            MustComplete = 0x0000004,
            LockDoorsAtPackageStart = 0x0000008,
            LockDoorsAtPackageEnd = 0x0000010,
            LockDoorsAtLocation = 0x0000020,
            UnlockDoorsAtPackageStart = 0x0000040,
            UnlockDoorsAtPackageEnd = 0x0000080,
            UnlockDoorsAtLocation = 0x0000100,
            ContinueIfPCNear = 0x0000200,
            OncePerDay = 0x0000400,
            Unused = 0x0000800,
            SkipFalloutBehavior = 0x0001000,
            AlwaysRun = 0x0002000,
            AlwaysSneak = 0x0020000,
            AllowSwimming = 0x0040000,
            AllowFalls = 0x0080000,
            ArmorUnequpped = 0x0100000,
            WeaponsUnequipped = 0x0200000,
            DefensiveCombat = 0x0400000,
            UseHorse = 0x0800000,
            NoIdleAnims = 0x1000000
        }

        public enum GeneralTypeEnum
        {
            Find = 0,
            Follow = 1,
            Escort = 2,
            Eat = 3,
            Sleep = 4,
            Wander = 5,
            Travel = 6,
            Accompany = 7,
            UseItemAt = 8,
            Ambush = 9,
            FleeNotCombat = 10,
            CastMagic = 11,
        }

        static partial void FillBinary_Flags_Custom(MutagenFrame frame, AIPackage item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (frame.Remaining == 8)
            {
                if (EnumBinaryTranslation<AIPackage.Flag>.Instance.Parse(
                    frame: frame.SpawnWithLength(4),
                    item: out AIPackage.Flag FlagsParse,
                    errorMask: errorMask))
                {
                    item.Flags = FlagsParse;
                }
                if (EnumBinaryTranslation<AIPackage.GeneralTypeEnum>.Instance.Parse(
                        frame: frame.SpawnWithLength(4),
                        item: out AIPackage.GeneralTypeEnum GeneralTypeParse,
                        errorMask: errorMask))
                {
                    item.GeneralType = GeneralTypeParse;
                }
            }
            else if (frame.Remaining == 4)
            {
                byte[] buff = new byte[4];
                frame.Reader.Read(buff, 0, 2);
                var subFrame = new MutagenFrame(
                    new BinaryMemoryReadStream(buff));
                if (EnumBinaryTranslation<AIPackage.Flag>.Instance.Parse(
                    frame: subFrame,
                    item: out AIPackage.Flag FlagsParse,
                    errorMask: errorMask))
                {
                    item.Flags = FlagsParse;
                }
                if (EnumBinaryTranslation<AIPackage.GeneralTypeEnum>.Instance.Parse(
                        frame: frame.SpawnWithLength(1),
                        item: out AIPackage.GeneralTypeEnum GeneralTypeParse,
                        errorMask: errorMask))
                {
                    item.GeneralType = GeneralTypeParse;
                }
                frame.Position += 1;
            }
            else
            {
                throw new ArgumentException($"Odd length for general AI field: {frame.Remaining}");
            }
        }

        static partial void WriteBinary_Flags_Custom(MutagenWriter writer, AIPackage item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            Mutagen.Bethesda.Binary.EnumBinaryTranslation<AIPackage.Flag>.Instance.Write(
                writer,
                item.Flags,
                length: 4,
                fieldIndex: (int)AIPackage_FieldIndex.Flags,
                errorMask: errorMask);
            Mutagen.Bethesda.Binary.EnumBinaryTranslation<AIPackage.GeneralTypeEnum>.Instance.Write(
                writer,
                item.GeneralType,
                length: 4,
                fieldIndex: (int)AIPackage_FieldIndex.GeneralType,
                errorMask: errorMask);
        }

        static partial void FillBinary_GeneralType_Custom(MutagenFrame frame, AIPackage item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
        }

        static partial void WriteBinary_GeneralType_Custom(MutagenWriter writer, AIPackage item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
        }
    }
}
