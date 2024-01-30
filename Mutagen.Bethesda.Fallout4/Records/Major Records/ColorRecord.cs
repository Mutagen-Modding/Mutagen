using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout4;

partial class ColorRecord
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x02,
    }
    
    [Flags]
    public enum Flag
    {
        Playable = 0x01,
        // Handled by abstract subclassing
        //RemappingIndex = 0x02,
        ExtendedLut = 0x04,
    }
}

partial class ColorRecordBinaryCreateTranslation
{
    public const int RemappingIndexFlag = 0x02;

    public static partial void FillBinaryDataCustom(
        MutagenFrame frame,
        IColorRecordInternal item,
        PreviousParse lastParsed)
    {
        var cnam = frame.ReadSubrecord(RecordTypes.CNAM, readSafe: true);
        var pos = frame.Position;
        if (!frame.SpawnAll().TryScanToRecord(RecordTypes.FNAM, out var fnam, ColorRecord_Registration.TriggerSpecs.AllRecordTypes))
        {
            throw new MalformedDataException("Did not find expected FNAM subrecord");
        }
        if (Enums.HasFlag(fnam.AsInt32(), RemappingIndexFlag))
        {
            item.Data = new ColorRemappingIndex()
            {
                Index = cnam.AsFloat(),
            };
        }
        else
        {
            item.Data = new ColorData()
            {
                Color = cnam.Content.ReadColor(Plugins.Binary.Translations.ColorBinaryType.Alpha)
            };
        }
        frame.Position = pos;
    }

    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IColorRecordInternal item, PreviousParse lastParsed)
    {
        var fnam = frame.ReadSubrecord(RecordTypes.FNAM);
        var val = fnam.AsInt32();
        Enums.SetFlag(ref val, RemappingIndexFlag, false);
        item.Flags = (ColorRecord.Flag)val;
    }
}

partial class ColorRecordBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(
        MutagenWriter writer,
        IColorRecordGetter item)
    {
        var data = item.Data;
        var flags = (int)item.Flags;
        flags = Enums.SetFlag(flags, ColorRecordBinaryCreateTranslation.RemappingIndexFlag, data is IColorRemappingIndexGetter);
        using (HeaderExport.Subrecord(writer, RecordTypes.CNAM))
        {
            data.WriteToBinary(writer);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.FNAM))
        {
            writer.Write(flags);
        }
    }

    public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IColorRecordGetter item)
    {
    }
}

partial class ColorRecordBinaryOverlay
{
    private int? _cnamLocation;
    private int? _fnamLocation;
    private int RawFlag
    {
        get
        {
            if (_fnamLocation == null)
            {
                throw new MalformedDataException("Did not find expected FNAM subrecord");
            }
            return BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_recordData, _fnamLocation.Value, _package.MetaData));
        }
    }

    public partial IAColorRecordDataGetter GetDataCustom()
    {
        if (_cnamLocation == null)
        {
            throw new MalformedDataException("Did not find expected CNAM subrecord");
        }

        var cnamMem = HeaderTranslation.ExtractSubrecordMemory(_recordData, _cnamLocation.Value, _package.MetaData);

        var flag = RawFlag;
        if (Enums.HasFlag(flag, ColorRecordBinaryCreateTranslation.RemappingIndexFlag))
        {
            return new ColorRemappingIndex()
            {
                Index = BinaryPrimitives.ReadSingleLittleEndian(cnamMem),
            };
        }
        else
        {
            return new ColorData()
            {
                Color = cnamMem.ReadColor(Plugins.Binary.Translations.ColorBinaryType.Alpha)
            };
        }
    }

    partial void DataCustomParse(
        OverlayStream stream,
        long finalPos,
        int offset)
    {
        _cnamLocation = (stream.Position - offset);
    }

    partial void FlagsCustomParse(
        OverlayStream stream,
        long finalPos,
        int offset)
    {
        _fnamLocation = (stream.Position - offset);
    }

    public partial ColorRecord.Flag GetFlagsCustom()
    {
        return (ColorRecord.Flag)Enums.SetFlag(RawFlag, ColorRecordBinaryCreateTranslation.RemappingIndexFlag, false);
    }
}
