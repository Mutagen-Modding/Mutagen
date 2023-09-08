using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Fallout4.Internals;

namespace Mutagen.Bethesda.Fallout4;

public partial class Furniture
{
    [Flags]
    public enum MajorFlag
    {
        IsPerch = 0x0000_0080,
        HasDistantLod = 0x0000_8000,
        RandomAnimStart = 0x0001_0000,
        IsMarker = 0x0080_0000,
        PowerArmor = 0x0200_0000,
        MustExitToTalk = 0x1000_0000,
        ChildCanUse = 0x2000_0000
    }

    [Flags]
    public enum Flag : uint
    {
        IgnoredBySandbox = 0x0000_0002,
        AllowAwakeSound = 0x0040_0000,
        EnterWithWeaponDrawn = 0x0080_0000,
        PlayAnimWhenFull = 0x0100_0000,
        DisablesActivation = 0x0200_0000,
        IsPerch = 0x0400_0000,
        MustExitToTalk = 0x0800_0000,
        UseStaticAvoidNode = 0x1000_0000,
        HasModel = 0x4000_0000,
        IsSleepFurniture = 0x8000_0000
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

    public enum Property
    {
    }
}

/// <summary>
/// Parsing for Furniture is fairly custom.  The 2nd flags subrecord has sit booleans, which are combined with both the
/// 'Markers' list and the 'Marker Entry Points' list from the binary data into one list of objects to be exposed
/// </summary>
partial class FurnitureBinaryCreateTranslation
{
    public const uint UpperFlagsMask = 0xFFC0_0000;
    public const uint NumSlots = 22;

    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IFurnitureInternal item, PreviousParse lastParsed)
    {
        var subFrame = frame.ReadSubrecord();
        // Read flags like normal
        item.Flags = (Furniture.Flag)BinaryPrimitives.ReadUInt16LittleEndian(subFrame.Content);
    }

    public static partial ParseResult FillBinaryFlags2Custom(MutagenFrame frame, IFurnitureInternal item, PreviousParse lastParsed)
    {
        // Clear out old stuff
        // This assumes flags will be parsed first.  Might need to be upgraded to not need that assumption
        item.MarkerParameters = null;
        item.Flags = FillBinaryFlags2(frame, (i) => GetNthMarker(item, i), item.Flags);
        return null;
    }

    public static Furniture.Flag FillBinaryFlags2(IMutagenReadStream stream, Func<int, FurnitureMarkerParameters> getter, Furniture.Flag? existingFlag)
    {
        var subFrame = stream.ReadSubrecord();
        uint raw = BinaryPrimitives.ReadUInt32LittleEndian(subFrame.Content);

        // Clear out upper bytes of existing flags
        var curFlags = (uint)(existingFlag ?? 0);
        curFlags &= ~UpperFlagsMask;

        // Add in new upper flags
        uint upperFlags = raw & UpperFlagsMask;
        var ret = (Furniture.Flag)(curFlags | upperFlags);

        // Create marker objects for sit flags
        uint markers = raw & 0x003F_FFFF;
        uint indexToCheck = 1;
        for (int i = 0; i < NumSlots; i++)
        {
            var has = Enums.HasFlag(markers, indexToCheck);
            indexToCheck <<= 1;
            if (!has) continue;
            var marker = getter(i);
            marker.Enabled = true;
        }
        return ret;
    }

    public static FurnitureMarkerParameters GetNthMarker(IFurnitureInternal item, int index)
    {
        if (item.MarkerParameters == null)
        {
            item.MarkerParameters = new ExtendedList<FurnitureMarkerParameters>();
        }
        if (!item.MarkerParameters.TryGet(index, out var marker))
        {
            while (item.MarkerParameters.Count <= index)
            {
                item.MarkerParameters.Add(new FurnitureMarkerParameters());
            }
            marker = item.MarkerParameters[^1];
        }
        return marker;
    }

    public static partial void FillBinaryEnabledEntryPointsCustom(MutagenFrame frame, IFurnitureInternal item, PreviousParse lastParsed)
    {
        item.EnabledEntryPoints = ParseBinaryEnabledEntryPointsCustom(frame);
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

    public static partial void FillBinaryMarkerParametersCustom(MutagenFrame frame, IFurnitureInternal item, PreviousParse lastParsed)
    {
        FillBinaryMarkers(frame, (i) => GetNthMarker(item, i));
    }

    public static void FillBinaryMarkers(MutagenFrame stream, Func<int, FurnitureMarkerParameters> getter)
    {
        var snam = stream.ReadSubrecordHeader(RecordTypes.SNAM);
        stream = stream.SpawnWithLength(snam.ContentLength);
        int i = 0;
        while (stream.Remaining > 0)
        {
            var marker = getter(i++);
            marker.CopyInFromBinary(stream, default);
        }
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
        var flags = (uint)(item.Flags ?? 0);
        // Trim out lower flags
        var exportFlags = flags & FurnitureBinaryCreateTranslation.UpperFlagsMask;

        var markers = item.MarkerParameters;
        if (markers != null)
        {
            // Enable appropriate sit markers
            uint indexToCheck = 1;
            foreach (var marker in markers)
            {
                exportFlags = Enums.SetFlag(exportFlags, indexToCheck, marker.Enabled);
                indexToCheck <<= 1;
            }
        }

        // Write out mashup of upper flags and sit markers
        using (HeaderExport.Subrecord(writer, RecordTypes.MNAM))
        {
            writer.Write(exportFlags);
        }
    }

    public static partial void WriteBinaryEnabledEntryPointsCustom(MutagenWriter writer, IFurnitureGetter item)
    {
        var entryPts = item.EnabledEntryPoints;
        if (entryPts == null) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.ENAM))
        {
            writer.Write(-1);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.NAM0))
        {
            writer.Write((short)0);
            EnumBinaryTranslation<Furniture.EntryPointType, MutagenFrame, MutagenWriter>.Instance.Write(
                writer,
                entryPts.Value,
                2);
        }
    }


    public static partial void WriteBinaryMarkerParametersCustom(MutagenWriter writer, IFurnitureGetter item)
    {
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IFurnitureMarkerParametersGetter>.Instance.Write(
            writer: writer,
            items: item.MarkerParameters,
            recordType: RecordTypes.SNAM,
            transl: (MutagenWriter subWriter, IFurnitureMarkerParametersGetter subItem, TypedWriteParams conv) =>
            {
                var Item = subItem;
                ((FurnitureMarkerParametersBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                    item: Item,
                    writer: subWriter,
                    translationParams: conv);
            });
    }
}

partial class FurnitureBinaryOverlay
{
    Furniture.Flag? _flags;
    public partial Furniture.Flag? GetFlagsCustom() => _flags;

    private ExtendedList<FurnitureMarkerParameters>? _markers;
    public IReadOnlyList<IFurnitureMarkerParametersGetter>? MarkerParameters => _markers;


    private Furniture.EntryPointType? _enabledEntryPoints;

    private FurnitureMarkerParameters GetNthMarker(int index)
    {
        if (this._markers == null)
        {
            this._markers = new ExtendedList<FurnitureMarkerParameters>();
        }
        if (!this._markers.TryGet(index, out var marker))
        {
            while (this._markers.Count <= index)
            {
                this._markers.Add(new FurnitureMarkerParameters());
            }
            marker = this._markers[^1];
        }
        return marker;
    }

    partial void FlagsCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        var subFrame = stream.ReadSubrecord();
        // Read flags like normal
        _flags = (Furniture.Flag)BinaryPrimitives.ReadUInt16LittleEndian(subFrame.Content);
    }

    public partial ParseResult Flags2CustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        this._flags = FurnitureBinaryCreateTranslation.FillBinaryFlags2(
            stream,
            this.GetNthMarker,
            this._flags);
        return null;
    }
            
    partial void EnabledEntryPointsCustomParse(
        OverlayStream stream,
        long finalPos,
        int offset)
    {
        _enabledEntryPoints = FurnitureBinaryCreateTranslation.ParseBinaryEnabledEntryPointsCustom(stream);
    }

    partial void MarkerParametersCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        FurnitureBinaryCreateTranslation.FillBinaryMarkers(new MutagenFrame(stream), GetNthMarker);
    }

    public partial Furniture.EntryPointType? GetEnabledEntryPointsCustom()
    {
        return _enabledEntryPoints;
    }
}