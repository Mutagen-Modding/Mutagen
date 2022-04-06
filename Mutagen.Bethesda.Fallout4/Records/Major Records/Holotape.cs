using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using System;
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
    public static partial ParseResult FillBinaryTypeParseCustom(MutagenFrame frame, IHolotapeInternal item)
    {
        var sub = frame.ReadSubrecord(RecordTypes.DNAM);
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

    public static partial ParseResult FillBinaryDataParseCustom(MutagenFrame frame, IHolotapeInternal item)
    {
        var sub = frame.ReadSubrecord();
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
                        prog.File = StringBinaryTranslation.Instance.Parse(frame);
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
            {
                using var header = HeaderExport.Subrecord(writer, RecordTypes.SNAM);
                FormKeyBinaryTranslation.Instance.Write(writer, sound.Sound.FormKey);
            }
                break;
            case IHolotapeVoiceGetter voice:
            {
                using var header = HeaderExport.Subrecord(writer, RecordTypes.SNAM);
                FormKeyBinaryTranslation.Instance.Write(writer, voice.Scene.FormKey);
            }
                break;
            case IHolotapeProgramGetter prog:
            {
                using var header = HeaderExport.Subrecord(writer, RecordTypes.PNAM);
                StringBinaryTranslation.Instance.Write(writer, prog.File);
            }
                break;
            case IHolotapeTerminalGetter term:
            {
                using var header = HeaderExport.Subrecord(writer, RecordTypes.SNAM);
                FormKeyBinaryTranslation.Instance.Write(writer, term.Terminal.FormKey);
            }
                break;
            default:
                break;
        }
    }
}

partial class HolotapeBinaryOverlay
{
    private int? _DataTypeLocation;
    private int? _DataContentLocation;

    public IAHolotapeDataGetter Data
    {
        get
        {
            if (!_DataTypeLocation.HasValue)
            {
                throw new MalformedDataException($"Did not parse {RecordTypes.DNAM} and so cannot provide Holotape data.");
            }
            if (!_DataContentLocation.HasValue)
            {
                throw new MalformedDataException($"Did not parse {RecordTypes.SNAM} and so cannot provide Holotape data.");
            }
            var typeMem = HeaderTranslation.ExtractSubrecordMemory(_data, _DataTypeLocation.Value, _package.MetaData.Constants);
            var dataMem = HeaderTranslation.ExtractSubrecordMemory(_data, _DataContentLocation.Value, _package.MetaData.Constants);
            var type = (Holotape.Types)typeMem[0];
            switch (type)
            {
                case Holotape.Types.Sound:
                    return new HolotapeSound()
                    {
                        Sound = FormKeyBinaryTranslation.Instance.Parse(dataMem, _package.MetaData.MasterReferences).AsLink<ISoundDescriptorGetter>()
                    };
                case Holotape.Types.Voice:
                    return new HolotapeVoice()
                    {
                        Scene = FormKeyBinaryTranslation.Instance.Parse(dataMem, _package.MetaData.MasterReferences).AsLink<ISceneGetter>()
                    };
                case Holotape.Types.Program:
                    return new HolotapeProgram()
                    {
                        File = StringBinaryTranslation.Instance.Parse(dataMem, _package.MetaData.Encodings.NonTranslated)
                    };
                case Holotape.Types.Terminal:
                    return new HolotapeTerminal()
                    {
                        Terminal = FormKeyBinaryTranslation.Instance.Parse(dataMem, _package.MetaData.MasterReferences).AsLink<ITerminalGetter>()
                    };
                default:
                    throw SubrecordException.Enrich(
                        new MalformedDataException($"Unknown Holotape type: {type}"),
                        RecordTypes.DNAM);
            }
        }
    }

    public partial ParseResult TypeParseCustomParse(OverlayStream stream, int offset)
    {
        _DataTypeLocation = (stream.Position - offset);
        return (int)Holotape_FieldIndex.Data;
    }

    public partial ParseResult DataParseCustomParse(OverlayStream stream, int offset)
    {
        _DataContentLocation = (stream.Position - offset);
        return (int)Holotape_FieldIndex.PickUpSound;
    }
}