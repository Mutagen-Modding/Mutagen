using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Fallout3;

public partial class Note
{
    public enum NoteType
    {
        Sound = 0,
        Text = 1,
        Image = 2,
    }
}

partial class NoteBinaryCreateTranslation
{
    private const byte VoiceTypeValue = 3;

    public static partial ParseResult FillBinaryTypeParseCustom(MutagenFrame frame, INoteInternal item, PreviousParse lastParsed)
    {
        var sub = frame.ReadSubrecordHeader(RecordTypes.DATA);
        var typeValue = frame.ReadUInt8();
        if (typeValue == VoiceTypeValue)
        {
            item.Data = new NoteVoice();
        }
        else
        {
            item.Data = new NoteStandard { Type = (Note.NoteType)typeValue };
        }
        return (int)Note_FieldIndex.DropSound;
    }

    public static partial ParseResult FillBinaryDataParseCustom(MutagenFrame frame, INoteInternal item, PreviousParse lastParsed)
    {
        var sub = frame.ReadSubrecordHeader();
        switch (sub.RecordTypeInt)
        {
            case RecordTypeInts.TNAM:
                switch (item.Data)
                {
                    case NoteStandard standard:
                        standard.Text = StringBinaryTranslation.Instance.Parse(frame, StringBinaryType.NullTerminate, parseWhole: true);
                        break;
                    case NoteVoice voice:
                        voice.Topic.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                        break;
                }
                break;
            case RecordTypeInts.SNAM:
                switch (item.Data)
                {
                    case NoteStandard standard:
                        standard.Sound.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                        break;
                    case NoteVoice voice:
                        voice.Speaker.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                        break;
                }
                break;
            default:
                throw new MalformedDataException($"Unexpected type when parsing Note data: {sub.RecordType}");
        }
        return (int)Note_FieldIndex.Data;
    }
}

partial class NoteBinaryWriteTranslation
{
    private const byte VoiceTypeValue = 3;

    public static partial void WriteBinaryTypeParseCustom(MutagenWriter writer, INoteGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.DATA);
        byte typeValue = item.Data switch
        {
            INoteStandardGetter standard => (byte)standard.Type,
            INoteVoiceGetter => VoiceTypeValue,
            _ => throw SubrecordException.Enrich(
                new NullReferenceException("Note data was null"),
                RecordTypes.DATA),
        };
        writer.Write(typeValue);
    }

    public static partial void WriteBinaryDataParseCustom(MutagenWriter writer, INoteGetter item)
    {
        switch (item.Data)
        {
            case INoteStandardGetter standard:
                if (standard.Text is { } textStr)
                {
                    using var header = HeaderExport.Subrecord(writer, RecordTypes.TNAM);
                    StringBinaryTranslation.Instance.Write(writer, textStr);
                }
                FormLinkBinaryTranslation.Instance.WriteNullable(writer, standard.Sound, RecordTypes.SNAM);
                break;
            case INoteVoiceGetter voice:
                FormLinkBinaryTranslation.Instance.WriteNullable(writer, voice.Topic, RecordTypes.TNAM);
                FormLinkBinaryTranslation.Instance.WriteNullable(writer, voice.Speaker, RecordTypes.SNAM);
                break;
        }
    }
}

partial class NoteBinaryOverlay
{
    private const byte VoiceTypeValue = 3;

    private int? _dataTypeLocation;
    private int? _dataContentTNAMLocation;
    private int? _dataContentSNAMLocation;

    public IANoteDataGetter Data
    {
        get
        {
            if (!_dataTypeLocation.HasValue)
            {
                throw new MalformedDataException($"Did not parse {RecordTypes.DATA} and so cannot provide Note data.");
            }
            var typeMem = HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataTypeLocation.Value, _package.MetaData.Constants);
            var typeValue = typeMem[0];
            if (typeValue == VoiceTypeValue)
            {
                var voice = new NoteVoice();
                if (_dataContentTNAMLocation.HasValue)
                {
                    voice.Topic.SetTo(
                        FormKeyBinaryTranslation.Instance.Parse(
                            HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentTNAMLocation.Value, _package.MetaData.Constants),
                            _package.MetaData.MasterReferences));
                }
                if (_dataContentSNAMLocation.HasValue)
                {
                    voice.Speaker.SetTo(
                        FormKeyBinaryTranslation.Instance.Parse(
                            HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentSNAMLocation.Value, _package.MetaData.Constants),
                            _package.MetaData.MasterReferences));
                }
                return voice;
            }
            else
            {
                var standard = new NoteStandard { Type = (Note.NoteType)typeValue };
                if (_dataContentTNAMLocation.HasValue)
                {
                    standard.Text = StringBinaryTranslation.Instance.Parse(
                        HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentTNAMLocation.Value, _package.MetaData.Constants),
                        _package.MetaData.Encodings.NonTranslated, parseWhole: true);
                }
                if (_dataContentSNAMLocation.HasValue)
                {
                    standard.Sound.SetTo(
                        FormKeyBinaryTranslation.Instance.Parse(
                            HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentSNAMLocation.Value, _package.MetaData.Constants),
                            _package.MetaData.MasterReferences));
                }
                return standard;
            }
        }
    }

    public partial ParseResult TypeParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        _dataTypeLocation = (stream.Position - offset);
        return (int)Note_FieldIndex.Data;
    }

    public partial ParseResult DataParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        var sub = stream.GetSubrecordHeader();
        if (sub.RecordType == RecordTypes.TNAM)
        {
            _dataContentTNAMLocation = (stream.Position - offset);
        }
        else if (sub.RecordType == RecordTypes.SNAM)
        {
            _dataContentSNAMLocation = (stream.Position - offset);
        }
        return (int)Note_FieldIndex.DropSound;
    }
}
