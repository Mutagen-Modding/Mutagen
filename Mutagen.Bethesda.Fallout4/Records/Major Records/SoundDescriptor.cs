using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

partial class SoundDescriptor
{
    public enum LoopType
    {
        None = 0,
        Loop = 0x08,
        EnvelopeFast = 0x10,
        EnvelopeSlow = 0x20,
    }
}

partial class SoundDescriptorBinaryCreateTranslation
{
    public enum DescriptorType : uint
    {
        Standard = 0x1EEF540A,
        Compound = 0x54651A43,
        AutoWeapon = 0xED157AE3 
    }
    
    public static partial ParseResult FillBinaryDataParseCustom(MutagenFrame frame, ISoundDescriptorInternal item, PreviousParse lastParsed)
    {
        if (item.Data == null)
        {
            item.Data = new SoundDescriptorStandardData();
        }

        frame.ReadSubrecordHeader(RecordTypes.BNAM);
        
        item.Data.CopyInFromBinary(frame);
        return null;
    }

    public static partial void FillBinaryDataCustom(MutagenFrame frame, ISoundDescriptorInternal item, PreviousParse lastParsed)
    {
        var cnam = frame.ReadSubrecord(RecordTypes.CNAM);
        var type = (DescriptorType)cnam.AsUInt32();
        item.Data = type switch
        {
            DescriptorType.Standard => new SoundDescriptorStandardData(),
            DescriptorType.Compound => new SoundDescriptorCompoundData(),
            DescriptorType.AutoWeapon => new SoundDescriptorAutoweaponData(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

partial class SoundDescriptorBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(MutagenWriter writer, ISoundDescriptorGetter item)
    {
        var type = item.Data switch
        {
            ISoundDescriptorStandardDataGetter _ => SoundDescriptorBinaryCreateTranslation.DescriptorType.Standard,
            ISoundDescriptorAutoweaponDataGetter _ => SoundDescriptorBinaryCreateTranslation.DescriptorType.AutoWeapon,
            ISoundDescriptorCompoundData _ => SoundDescriptorBinaryCreateTranslation.DescriptorType.Compound,
            _ => throw new NotImplementedException()
        };
        
        using (HeaderExport.Subrecord(writer, RecordTypes.CNAM))
        {
            writer.Write((uint)type);
        }
    }

    public static partial void WriteBinaryDataParseCustom(MutagenWriter writer, ISoundDescriptorGetter item)
    {
        if (item.Data is { } data)
        {
            if (data is ISoundDescriptorCompoundDataGetter) return;
            using (HeaderExport.Subrecord(writer, RecordTypes.BNAM))
            {
                data.WriteToBinary(writer);
            }
        }
    }
}

partial class SoundDescriptorBinaryOverlay
{
    private ASoundDescriptor? _descriptor;

    partial void DataCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        var cnam = stream.ReadSubrecord(RecordTypes.CNAM).AsUInt32();
        var type = (SoundDescriptorBinaryCreateTranslation.DescriptorType)cnam;
        _descriptor = type switch
        {
            SoundDescriptorBinaryCreateTranslation.DescriptorType.Standard => new SoundDescriptorStandardData(),
            SoundDescriptorBinaryCreateTranslation.DescriptorType.Compound => new SoundDescriptorCompoundData(),
            SoundDescriptorBinaryCreateTranslation.DescriptorType.AutoWeapon => new SoundDescriptorAutoweaponData(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public partial IASoundDescriptorGetter? GetDataCustom() => _descriptor;

    public partial ParseResult DataParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        if (_descriptor == null)
        {
            _descriptor = new SoundDescriptorStandardData();
        }

        stream.ReadSubrecordHeader(RecordTypes.BNAM);
        _descriptor.CopyInFromBinary(
            new MutagenFrame(stream));
        return null;
    }
}
