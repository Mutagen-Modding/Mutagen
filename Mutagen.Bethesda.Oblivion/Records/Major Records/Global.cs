using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins.Binary.Headers;

namespace Mutagen.Bethesda.Oblivion;

public partial interface IGlobalGetter
{
    char TypeChar { get; }
}

public partial class Global : GlobalCustomParsing.IGlobalCommon
{
    public abstract float? RawFloat { get; set; }

    char IGlobalGetter.TypeChar => throw new NotImplementedException();

    public static Global CreateFromBinary(
        MutagenFrame frame,
        TypedParseParams translationParams)
    {
        return GlobalCustomParsing.Create<Global>(
            frame,
            getter: (f, triggerChar) =>
            {
                switch (triggerChar)
                {
                    case GlobalInt.TRIGGER_CHAR:
                        return GlobalInt.CreateFromBinary(f);
                    case GlobalShort.TRIGGER_CHAR:
                        return GlobalShort.CreateFromBinary(f);
                    case GlobalFloat.TRIGGER_CHAR:
                        return GlobalFloat.CreateFromBinary(f);
                    default:
                        return GlobalUnknown.CreateFromBinary(f);
                }
            });
    }
}

partial class GlobalBinaryWriteTranslation
{
    public static partial void WriteBinaryTypeCharCustom(
        MutagenWriter writer,
        IGlobalGetter item)
    {
        CharBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            item.TypeChar,
            header: RecordTypes.FNAM);
    }
}

partial class GlobalBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryTypeCharCustom(MutagenFrame frame, IGlobalInternal item, PreviousParse lastParsed)
    {
        return null;
    }
}

abstract partial class GlobalBinaryOverlay
{
    public abstract float? RawFloat { get; }
    char IGlobalGetter.TypeChar => throw new NotImplementedException();

    public static IGlobalGetter GlobalFactory(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        TypedParseParams translationParams)
    {
        var majorFrame = package.MetaData.Constants.MajorRecord(stream.RemainingMemory);
        var globalChar = GlobalCustomParsing.GetGlobalChar(majorFrame);
        switch (globalChar)
        {
            case GlobalInt.TRIGGER_CHAR:
                return GlobalIntBinaryOverlay.GlobalIntFactory(
                    stream,
                    package,
                    translationParams);
            case GlobalShort.TRIGGER_CHAR:
                return GlobalShortBinaryOverlay.GlobalShortFactory(
                    stream,
                    package,
                    translationParams);
            case GlobalFloat.TRIGGER_CHAR:
                return GlobalFloatBinaryOverlay.GlobalFloatFactory(
                    stream,
                    package,
                    translationParams);
            default:
                return GlobalUnknownBinaryOverlay.GlobalUnknownFactory(
                    stream,
                    package);
        }
    }

    public partial ParseResult TypeCharCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        return null;
    }
}