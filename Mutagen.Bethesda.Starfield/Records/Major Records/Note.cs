using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Starfield;

partial class Note
{
    public enum Types
    {
        Voice = 1,
        Program = 2,
        Terminal = 3
    };
}

partial class NoteBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryTypeParseCustom(MutagenFrame frame, INoteInternal item, PreviousParse lastParsed)
    {
        var sub = frame.ReadSubrecordHeader(RecordTypes.DNAM);
        var type = EnumBinaryTranslation<Note.Types, MutagenFrame, MutagenWriter>.Instance.Parse(frame, 1);
        switch (type)
        {
            case Note.Types.Voice:
                item.Data = new NoteVoice();
                break;
            case Note.Types.Program:
                item.Data = new NoteProgram();
                break;
            case Note.Types.Terminal:
                item.Data = new NoteTerminal();
                break;
            default:
                throw SubrecordException.Enrich(
                    new MalformedDataException($"Unknown Note type: {type}"),
                    RecordTypes.DNAM);
        }
        return (int)Note_FieldIndex.DropdownSound;
    }

    public static partial ParseResult FillBinaryDataParseCustom(MutagenFrame frame, INoteInternal item, PreviousParse lastParsed)
    {
        var sub = frame.ReadSubrecordHeader();
        switch (sub.RecordTypeInt)
        {
            case RecordTypeInts.SNAM:
                switch (item.Data)
                {
                    case NoteVoice voice:
                        voice.Scene.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                        break;
                    case NoteProgram _:
                        // Discard
                        break;
                    case NoteTerminal term:
                        term.Terminal.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                        break;
                    default:
                        throw SubrecordException.Enrich(
                            new MalformedDataException($"Note type was not parsed first"),
                            RecordTypes.SNAM);
                }
                break;
            case RecordTypeInts.PNAM:
                switch (item.Data)
                {
                    case NoteProgram prog:
                        prog.File = StringBinaryTranslation.Instance.Parse(frame, StringBinaryType.NullTerminate);
                        break;
                    case NoteVoice _:
                    case NoteTerminal _:
                        // Discard
                        break;
                    default:
                        throw SubrecordException.Enrich(
                            new MalformedDataException($"Note type was not parsed first"),
                            RecordTypes.SNAM);
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
    public static partial void WriteBinaryTypeParseCustom(MutagenWriter writer, INoteGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.DNAM);

        Note.Types type;
        switch (item.Data)
        {
            case INoteVoiceGetter _:
                type = Note.Types.Voice;
                break;
            case INoteProgramGetter _:
                type = Note.Types.Program;
                break;
            case INoteTerminalGetter _:
                type = Note.Types.Terminal;
                break;
            default:
                throw SubrecordException.Enrich(
                    new NullReferenceException($"Holotype data was null"),
                    RecordTypes.SNAM);
        }

        EnumBinaryTranslation<Note.Types, MutagenFrame, MutagenWriter>.Instance.Write(writer, type, 1);
    }

    public static partial void WriteBinaryDataParseCustom(MutagenWriter writer, INoteGetter item)
    {
        switch (item.Data)
        {
            case INoteVoiceGetter voice:
                FormLinkBinaryTranslation.Instance.WriteNullable(writer, voice.Scene, RecordTypes.SNAM);
                break;
            case INoteProgramGetter prog:
                if (prog.File is { } file)
                {
                    using var header = HeaderExport.Subrecord(writer, RecordTypes.PNAM);
                    StringBinaryTranslation.Instance.Write(writer, file);
                }
                break;
            case INoteTerminalGetter term:
                FormLinkBinaryTranslation.Instance.WriteNullable(writer, term.Terminal, RecordTypes.SNAM);
                break;
            default:
                break;
        }
    }
}

partial class NoteBinaryOverlay
{
    private int? _dataTypeLocation;
    private int? _dataContentLocation;

    public IANoteDataGetter Data
    {
        get
        {
            if (!_dataTypeLocation.HasValue)
            {
                throw new MalformedDataException($"Did not parse {RecordTypes.DNAM} and so cannot provide Note data.");
            }
            var typeMem = HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataTypeLocation.Value, _package.MetaData.Constants);
            var type = (Note.Types)typeMem[0];
            switch (type)
            {
                case Note.Types.Voice:
                    var voice = new NoteVoice();
                    if (_dataContentLocation.HasValue)
                    {
                        voice.Scene.SetTo(
                            FormKeyBinaryTranslation.Instance.Parse(
                                HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentLocation.Value, _package.MetaData.Constants), 
                                _package.MetaData.MasterReferences));
                    }
                    return voice;
                case Note.Types.Program:
                    var prog = new NoteProgram();
                    if (_dataContentLocation.HasValue)
                    {
                        prog.File = StringBinaryTranslation.Instance.Parse(HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentLocation.Value, _package.MetaData.Constants), _package.MetaData.Encodings.NonTranslated);
                    }
                    return prog;
                case Note.Types.Terminal:
                    var term = new NoteTerminal();
                    if (_dataContentLocation.HasValue)
                    {
                        term.Terminal.SetTo(
                            FormKeyBinaryTranslation.Instance.Parse(
                                HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentLocation.Value, _package.MetaData.Constants), 
                                _package.MetaData.MasterReferences));
                    }
                    return term;
                default:
                    throw SubrecordException.Enrich(
                        new MalformedDataException($"Unknown Note type: {type}"),
                        RecordTypes.DNAM);
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
        _dataContentLocation = (stream.Position - offset);
        return (int)Note_FieldIndex.DropdownSound;
    }
}