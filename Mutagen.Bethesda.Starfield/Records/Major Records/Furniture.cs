using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

partial class Furniture
{
    [Flags]
    public enum MajorFlag
    {
        HasContainer = 0x0000_0004,
        IsPerch = 0x0000_0080,
        HasDistantLod = 0x0000_8000,
        RandomAnimStart = 0x0001_0000,
        IsMarker = 0x0080_0000,
        PowerArmor = 0x0200_0000,
        MustExitToTalk = 0x1000_0000,
        ChildCanUse = 0x2000_0000
    }

    [Flags]
    public enum Flag : ulong
    {
        IgnoredBySandbox = 0x0000_0000_0000_0002,
        InteractionPoint0 = 0x0000_0001_0000_0000,
        InteractionPoint1 = 0x0000_0002_0000_0000,
        InteractionPoint2 = 0x0000_0004_0000_0000,
        InteractionPoint3 = 0x0000_0008_0000_0000,
        InteractionPoint4 = 0x0000_0010_0000_0000,
        InteractionPoint5 = 0x0000_0020_0000_0000,
        InteractionPoint6 = 0x0000_0040_0000_0000,
        InteractionPoint7 = 0x0000_0080_0000_0000,
        InteractionPoint8 = 0x0000_0100_0000_0000,
        InteractionPoint9 = 0x0000_0200_0000_0000,
        InteractionPoint10 = 0x0000_0400_0000_0000,
        InteractionPoint11 = 0x0000_0800_0000_0000,
        InteractionPoint12 = 0x0000_1000_0000_0000,
        InteractionPoint13 = 0x0000_2000_0000_0000,
        InteractionPoint14 = 0x0000_4000_0000_0000,
        InteractionPoint15 = 0x0000_8000_0000_0000,
        InteractionPoint16 = 0x0001_0000_0000_0000,
        InteractionPoint17 = 0x0002_0000_0000_0000,
        InteractionPoint18 = 0x0004_0000_0000_0000,
        InteractionPoint19 = 0x0008_0000_0000_0000,
        InteractionPoint20 = 0x0010_0000_0000_0000,
        InteractionPoint21 = 0x0020_0000_0000_0000,
        AllowAwakeSound = 0x0040_0000_0000_0000,
        EnterWithWeaponDrawn = 0x0080_0000_0000_0000,
        PlayAnimWhenFull = 0x0100_0000_0000_0000,
        DisablesActivation = 0x0200_0000_0000_0000,
        IsPerch = 0x0400_0000_0000_0000,
        MustExitToTalk = 0x0800_0000_0000_0000,
        UseStaticAvoidNode = 0x1000_0000_0000_0000,
        HasModel = 0x4000_0000_0000_0000,
        IsSleepFurniture = 0x8000_0000_0000_0000
    }
    
    public enum BenchTypes
    {
        None = 0,
        CreateObject = 1,
        Weapons = 2,
        Enchanting = 3,
        EnchantingExperiment = 4,
        Alchemy = 5,
        AlchemyExperiment = 6,
        Armor = 7,
        PowerArmor = 8,
        RobotMod = 9,
        Research = 11,
    }

    [Flags]
    public enum EntryPointType
    {
        Front = 0x01,
        Behind = 0x02,
        Right = 0x04,
        Left = 0x08,
        Up = 0x10
    }

    [Flags]
    public enum EntryParameterType
    {
        Front = 0x01,
        Behind = 0x02,
        Right = 0x04,
        Left = 0x08,
        Other = 0x10
    }

    [Flags]
    public enum AnimationType
    {
        Sit = 1,
        Lay = 2,
        Lean = 4,
    }
}


/// <summary>
/// Parsing for Furniture is fairly custom.  The 2nd flags subrecord has sit booleans, which are combined with both the
/// 'Markers' list and the 'Marker Entry Points' list from the binary data into one list of objects to be exposed
/// </summary>
partial class FurnitureBinaryCreateTranslation
{
    public const uint UpperFlagsMask = 0xFFFF_0000;

    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IFurnitureInternal item, PreviousParse lastParsed)
    {
        var subFrame = frame.ReadSubrecord();
        // Read flags like normal
        item.Flags = (Furniture.Flag)BinaryPrimitives.ReadUInt16LittleEndian(subFrame.Content);
    }

    public static partial ParseResult FillBinaryFlags2Custom(MutagenFrame frame, IFurnitureInternal item, PreviousParse lastParsed)
    {
        var subFrame = frame.ReadSubrecord();
        
        // Clear out upper flags
        item.Flags &= ((Furniture.Flag)0x00000000FFFFFFFF);

        // Set upper flags
        ulong flags2 = subFrame.AsUInt32();
        flags2 <<= 32;
        item.Flags |= ((Furniture.Flag)flags2);
        return null;
    }

    public static Furniture.EntryPointType ParseBinaryEnabledEntryPointsCustom<TReader>(TReader frame)
        where TReader : IMutagenReadStream
    {
        var enam = frame.ReadSubrecord(RecordTypes.ENAM);
        var index = enam.AsInt32();
        if (index != -1)
        {
            throw new ArgumentException($"Unexpected ENAM index: {index}");
        }
        var name0 = frame.ReadSubrecord(RecordTypes.NAM0);
        var zeros = BinaryPrimitives.ReadInt16LittleEndian(name0.Content);
        if (zeros != 0)
        {
            throw new ArgumentException($"Unexpected non-zero NAM0 data: {zeros}");
        }
        return (Furniture.EntryPointType)BinaryPrimitives.ReadInt16LittleEndian(name0.Content.Slice(2));
    }
}

partial class FurnitureBinaryWriteTranslation
{
    public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IFurnitureGetter item)
    {
        var flags = (uint)(item.Flags ?? 0);
        // Trim out upper flags
        var normalFlags = flags & ~FurnitureBinaryCreateTranslation.UpperFlagsMask;
        using (HeaderExport.Subrecord(writer, RecordTypes.FNAM))
        {
            writer.Write(checked((ushort)normalFlags));
        }
    }

    public static partial void WriteBinaryFlags2Custom(MutagenWriter writer, IFurnitureGetter item)
    {
        var flagsEnum = item.Flags;
        if (flagsEnum == null) return;
        
        // Write out mashup of upper flags and sit markers
        using (HeaderExport.Subrecord(writer, RecordTypes.MNAM))
        {
            ulong flags = (ulong)flagsEnum;
            flags >>= 32;
            writer.Write((uint)flags);
        }
    }
}

partial class FurnitureBinaryOverlay
{
    Furniture.Flag? _flags;
    public partial Furniture.Flag? GetFlagsCustom() => _flags;

    partial void FlagsCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        var subFrame = stream.ReadSubrecord();
        // Read flags like normal
        _flags = (Furniture.Flag)BinaryPrimitives.ReadUInt16LittleEndian(subFrame.Content);
    }

    public partial ParseResult Flags2CustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        var subFrame = stream.ReadSubrecord();
        
        // Clear out upper flags
        this._flags &= ((Furniture.Flag)0x00000000FFFFFFFF);

        // Set upper flags
        ulong flags2 = subFrame.AsUInt32();
        flags2 <<= 32;
        this._flags |= ((Furniture.Flag)flags2);
        return null;
    }
}