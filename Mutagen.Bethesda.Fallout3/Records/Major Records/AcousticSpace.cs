using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

public partial class AcousticSpace
{
    public enum EnvironmentTypeEnum
    {
        None,
        Default,
        Generic,
        PaddedCell,
        Room,
        Bathroom,
        Livingroom,
        StoneRoom,
        Auditorium,
        Concerthall,
        Cave,
        Arena,
        Hangar,
        CarpetedHallway,
        Hallway,
        StoneCorridor,
        Alley,
        Forest,
        City,
        Mountains,
        Quarry,
        Plain,
        Parkinglot,
        Sewerpipe,
        Underwater,
        SmallRoom,
        MediumRoom,
        LargeRoom,
        MediumHall,
        LargeHall,
        Plate
    }
}

partial class AcousticSpaceBinaryCreateTranslation
{
    public static partial ParseResult FillBinarySoundLoopCustom(
        MutagenFrame frame,
        IAcousticSpaceInternal item,
        PreviousParse lastParsed)
    {
        var reader = frame.Reader;
        if (!reader.TryReadSubrecord(RecordTypes.SNAM, out var subFrame)) return null;
        item.DawnDefaultLoop.SetTo(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, reader.MetaData.MasterReferences));

        if (!reader.TryReadSubrecord(RecordTypes.SNAM, out subFrame)) return (int)AcousticSpace_FieldIndex.DawnDefaultLoop;
        item.Afternoon.SetTo(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, reader.MetaData.MasterReferences));

        if (!reader.TryReadSubrecord(RecordTypes.SNAM, out subFrame)) return (int)AcousticSpace_FieldIndex.Afternoon;
        item.Dusk.SetTo(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, reader.MetaData.MasterReferences));

        if (!reader.TryReadSubrecord(RecordTypes.SNAM, out subFrame)) return (int)AcousticSpace_FieldIndex.Dusk;
        item.Night.SetTo(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, reader.MetaData.MasterReferences));

        if (!reader.TryReadSubrecord(RecordTypes.SNAM, out subFrame)) return (int)AcousticSpace_FieldIndex.Night;
        item.Walla.SetTo(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, reader.MetaData.MasterReferences));

        return (int)AcousticSpace_FieldIndex.Walla;
    }
}

partial class AcousticSpaceBinaryWriteTranslation
{
    public static partial void WriteBinarySoundLoopCustom(
        MutagenWriter writer, IAcousticSpaceGetter item)
    {
        if (writer.MetaData.ModHeaderVersion!.Value >= 1.32f)
        {
            // FNV: always write all 5 SNAMs
            WriteSnam(writer, item.DawnDefaultLoop);
            WriteSnam(writer, item.Afternoon);
            WriteSnam(writer, item.Dusk);
            WriteSnam(writer, item.Night);
            WriteSnam(writer, item.Walla);
        }
        else
        {
            // FO3: write single SNAM only if non-null
            if (!item.DawnDefaultLoop.IsNull)
            {
                WriteSnam(writer, item.DawnDefaultLoop);
            }
        }
    }

    private static void WriteSnam(MutagenWriter writer, IFormLinkGetter<ISoundGetter> link)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.SNAM))
        {
            FormKeyBinaryTranslation.Instance.Write(writer, link);
        }
    }
}

partial class AcousticSpaceBinaryOverlay
{
    private IFormLinkGetter<ISoundGetter> _dawnDefaultLoop = FormLink<ISoundGetter>.Null;
    private IFormLinkGetter<ISoundGetter> _afternoon = FormLink<ISoundGetter>.Null;
    private IFormLinkGetter<ISoundGetter> _dusk = FormLink<ISoundGetter>.Null;
    private IFormLinkGetter<ISoundGetter> _night = FormLink<ISoundGetter>.Null;
    private IFormLinkGetter<ISoundGetter> _walla = FormLink<ISoundGetter>.Null;

    public IFormLinkGetter<ISoundGetter> DawnDefaultLoop => _dawnDefaultLoop;
    public IFormLinkGetter<ISoundGetter> Afternoon => _afternoon;
    public IFormLinkGetter<ISoundGetter> Dusk => _dusk;
    public IFormLinkGetter<ISoundGetter> Night => _night;
    public IFormLinkGetter<ISoundGetter> Walla => _walla;

    public partial ParseResult SoundLoopCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        if (!stream.TryReadSubrecord(RecordTypes.SNAM, out var subFrame)) return null;
        _dawnDefaultLoop = new FormLink<ISoundGetter>(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, _package.MetaData.MasterReferences));

        if (!stream.TryReadSubrecord(RecordTypes.SNAM, out subFrame)) return (int)AcousticSpace_FieldIndex.DawnDefaultLoop;
        _afternoon = new FormLink<ISoundGetter>(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, _package.MetaData.MasterReferences));

        if (!stream.TryReadSubrecord(RecordTypes.SNAM, out subFrame)) return (int)AcousticSpace_FieldIndex.Afternoon;
        _dusk = new FormLink<ISoundGetter>(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, _package.MetaData.MasterReferences));

        if (!stream.TryReadSubrecord(RecordTypes.SNAM, out subFrame)) return (int)AcousticSpace_FieldIndex.Dusk;
        _night = new FormLink<ISoundGetter>(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, _package.MetaData.MasterReferences));

        if (!stream.TryReadSubrecord(RecordTypes.SNAM, out subFrame)) return (int)AcousticSpace_FieldIndex.Night;
        _walla = new FormLink<ISoundGetter>(FormKeyBinaryTranslation.Instance.Parse(subFrame.Content, _package.MetaData.MasterReferences));

        return (int)AcousticSpace_FieldIndex.Walla;
    }
}
