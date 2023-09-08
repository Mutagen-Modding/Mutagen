using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Oblivion;

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

    public enum Types
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
}

partial class AIPackageDataBinaryCreateTranslation
{
    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IAIPackageData item)
    {
        if (frame.Remaining == 8)
        {
            var span = frame.Reader.ReadSpan(8);
            item.Flags = Enums<AIPackage.Flag>.Convert(BinaryPrimitives.ReadUInt32LittleEndian(span));
            item.Type = Enums<AIPackage.Types>.Convert(BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4)));
        }
        else if (frame.Remaining == 4)
        {
            var span = frame.Reader.ReadSpan(4);
            item.Flags = Enums<AIPackage.Flag>.Convert(BinaryPrimitives.ReadUInt16LittleEndian(span));
            item.Type = Enums<AIPackage.Types>.Convert(span[2]);
        }
        else
        {
            throw new ArgumentException($"Odd length for general AI field: {frame.Remaining}");
        }
    }

    public static partial void FillBinaryTypeCustom(MutagenFrame frame, IAIPackageData item)
    {
    }
}

partial class AIPackageDataBinaryWriteTranslation
{
    public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IAIPackageDataGetter item)
    {
        EnumBinaryTranslation<AIPackage.Flag, MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            item.Flags,
            length: 4);
        EnumBinaryTranslation<AIPackage.Types, MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            item.Type,
            length: 4);
    }

    public static partial void WriteBinaryTypeCustom(MutagenWriter writer, IAIPackageDataGetter item)
    {
    }
}

partial class AIPackageDataBinaryOverlay
{
    public partial AIPackage.Flag GetFlagsCustom(int location)
    {
        if (_structData.Length > 4)
        {
            return Enums<AIPackage.Flag>.Convert(BinaryPrimitives.ReadUInt32LittleEndian(_structData));
        }
        else
        {
            return Enums<AIPackage.Flag>.Convert(BinaryPrimitives.ReadUInt16LittleEndian(_structData));
        }
    }

    public partial AIPackage.Types GetTypeCustom(int location)
    {
        if (_structData.Length > 4)
        {
            return Enums<AIPackage.Types>.Convert(BinaryPrimitives.ReadUInt32LittleEndian(_structData.Slice(4)));
        }
        else
        {
            return Enums<AIPackage.Types>.Convert(_structData[2]);
        }
    }
}