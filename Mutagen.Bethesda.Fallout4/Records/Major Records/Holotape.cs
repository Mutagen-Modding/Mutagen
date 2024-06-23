using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using Mutagen.Bethesda.Fallout4.Internals;

namespace Mutagen.Bethesda.Fallout4;

public partial class Holotape
{
    public enum Types
    {
        Sound,
        Voice,
        Program,
        Terminal
    };
}

partial class HolotapeBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryTypeParseCustom(MutagenFrame frame, IHolotapeInternal item, PreviousParse lastParsed)
    {
        var sub = frame.ReadSubrecordHeader(RecordTypes.DNAM);
        var type = EnumBinaryTranslation<Holotape.Types, MutagenFrame, MutagenWriter>.Instance.Parse(frame, 1);
        switch (type)
        {
            case Holotape.Types.Sound:
                item.Data = new HolotapeSound();
                break;
            case Holotape.Types.Voice:
                item.Data = new HolotapeVoice();
                break;
            case Holotape.Types.Program:
                item.Data = new HolotapeProgram();
                break;
            case Holotape.Types.Terminal:
                item.Data = new HolotapeTerminal();
                break;
            default:
                throw SubrecordException.Enrich(
                    new MalformedDataException($"Unknown Holotape type: {type}"),
                    RecordTypes.DNAM);
        }
        return (int)Holotape_FieldIndex.PickUpSound;
    }

    public static partial ParseResult FillBinaryDataParseCustom(MutagenFrame frame, IHolotapeInternal item, PreviousParse lastParsed)
    {
        var sub = frame.ReadSubrecordHeader();
        switch (sub.RecordTypeInt)
        {
            case RecordTypeInts.SNAM:
                switch (item.Data)
                {
                    case HolotapeSound sound:
                        sound.Sound.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                        break;
                    case HolotapeVoice voice:
                        voice.Scene.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                        break;
                    case HolotapeProgram _:
                        // Discard
                        break;
                    case HolotapeTerminal term:
                        term.Terminal.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
                        break;
                    default:
                        throw SubrecordException.Enrich(
                            new MalformedDataException($"Holotape type was not parsed first"),
                            RecordTypes.SNAM);
                }
                break;
            case RecordTypeInts.PNAM:
                switch (item.Data)
                {
                    case HolotapeProgram prog:
                        prog.File = StringBinaryTranslation.Instance.Parse(frame, StringBinaryType.NullTerminate);
                        break;
                    case HolotapeSound _:
                    case HolotapeVoice _:
                    case HolotapeTerminal _:
                        // Discard
                        break;
                    default:
                        throw SubrecordException.Enrich(
                            new MalformedDataException($"Holotape type was not parsed first"),
                            RecordTypes.SNAM);
                }
                break;
            default:
                throw new MalformedDataException($"Unexpected type when parsing Holotape data: {sub.RecordType}");
        }
        return (int)Holotape_FieldIndex.Data;
    }
}

partial class HolotapeBinaryWriteTranslation
{
    public static partial void WriteBinaryTypeParseCustom(MutagenWriter writer, IHolotapeGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.DNAM);

        Holotape.Types type;
        switch (item.Data)
        {
            case IHolotapeSoundGetter _:
                type = Holotape.Types.Sound;
                break;
            case IHolotapeVoiceGetter _:
                type = Holotape.Types.Voice;
                break;
            case IHolotapeProgramGetter _:
                type = Holotape.Types.Program;
                break;
            case IHolotapeTerminalGetter _:
                type = Holotape.Types.Terminal;
                break;
            default:
                throw SubrecordException.Enrich(
                    new NullReferenceException($"Holotype data was null"),
                    RecordTypes.SNAM);
        }

        EnumBinaryTranslation<Holotape.Types, MutagenFrame, MutagenWriter>.Instance.Write(writer, type, 1);
    }

    public static partial void WriteBinaryDataParseCustom(MutagenWriter writer, IHolotapeGetter item)
    {
        switch (item.Data)
        {
            case IHolotapeSoundGetter sound:
                FormLinkBinaryTranslation.Instance.WriteNullable(writer, sound.Sound, RecordTypes.SNAM);
                break;
            case IHolotapeVoiceGetter voice:
                FormLinkBinaryTranslation.Instance.WriteNullable(writer, voice.Scene, RecordTypes.SNAM);
                break;
            case IHolotapeProgramGetter prog:
                if (prog.File is { } file)
                {
                    using var header = HeaderExport.Subrecord(writer, RecordTypes.PNAM);
                    StringBinaryTranslation.Instance.Write(writer, file);
                }
                break;
            case IHolotapeTerminalGetter term:
                FormLinkBinaryTranslation.Instance.WriteNullable(writer, term.Terminal, RecordTypes.SNAM);
                break;
            default:
                break;
        }
    }
}

partial class HolotapeBinaryOverlay
{
    private int? _dataTypeLocation;
    private int? _dataContentLocation;

    public IAHolotapeDataGetter Data
    {
        get
        {
            if (!_dataTypeLocation.HasValue)
            {
                throw new MalformedDataException($"Did not parse {RecordTypes.DNAM} and so cannot provide Holotape data.");
            }
            var typeMem = HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataTypeLocation.Value, _package.MetaData.Constants);
            var type = (Holotape.Types)typeMem[0];
            switch (type)
            {
                case Holotape.Types.Sound:
                    var sound = new HolotapeSound();
                    if (_dataContentLocation.HasValue)
                    {
                        sound.Sound.SetTo(
                            FormKeyBinaryTranslation.Instance.Parse(
                                HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentLocation.Value, _package.MetaData.Constants), 
                                _package.MetaData.MasterReferences.Raw));
                    }
                    return sound;
                case Holotape.Types.Voice:
                    var voice = new HolotapeVoice();
                    if (_dataContentLocation.HasValue)
                    {
                        voice.Scene.SetTo(
                            FormKeyBinaryTranslation.Instance.Parse(
                                HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentLocation.Value, _package.MetaData.Constants), 
                                _package.MetaData.MasterReferences.Raw));
                    }
                    return voice;
                case Holotape.Types.Program:
                    var prog = new HolotapeProgram();
                    if (_dataContentLocation.HasValue)
                    {
                        prog.File = StringBinaryTranslation.Instance.Parse(HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentLocation.Value, _package.MetaData.Constants), _package.MetaData.Encodings.NonTranslated);
                    }
                    return prog;
                case Holotape.Types.Terminal:
                    var term = new HolotapeTerminal();
                    if (_dataContentLocation.HasValue)
                    {
                        term.Terminal.SetTo(
                            FormKeyBinaryTranslation.Instance.Parse(
                                HeaderTranslation.ExtractSubrecordMemory(_recordData, _dataContentLocation.Value, _package.MetaData.Constants), 
                                _package.MetaData.MasterReferences.Raw));
                    }
                    return term;
                default:
                    throw SubrecordException.Enrich(
                        new MalformedDataException($"Unknown Holotape type: {type}"),
                        RecordTypes.DNAM);
            }
        }
    }

    public partial ParseResult TypeParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        _dataTypeLocation = (stream.Position - offset);
        return (int)Holotape_FieldIndex.Data;
    }

    public partial ParseResult DataParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        _dataContentLocation = (stream.Position - offset);
        return (int)Holotape_FieldIndex.PickUpSound;
    }
}