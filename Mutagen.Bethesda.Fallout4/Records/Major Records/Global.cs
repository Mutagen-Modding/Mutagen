using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Fallout4.Internals;

namespace Mutagen.Bethesda.Fallout4;

public partial interface IGlobalGetter
{
    char TypeChar { get; }
}

public partial class Global : GlobalCustomParsing.IGlobalCommon
{
    public abstract float? RawFloat { get; set; }
    
    char IGlobalGetter.TypeChar => throw new NotImplementedException();

    [Flags]
    public enum MajorFlag
    {
        Constant = 0x0000_0040
    }

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
                    case GlobalBool.TRIGGER_CHAR:
                        return GlobalBool.CreateFromBinary(f);
                    case GlobalFloat.TRIGGER_CHAR:
                    {
                        var ret = GlobalFloat.CreateFromBinary(f);
                        ret.OutputChar = true;
                        return ret;
                    }
                    default:
                        return GlobalFloat.CreateFromBinary(f);
                }
            });
    }
}


partial class GlobalBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryTypeCharCustom(MutagenFrame frame, IGlobalInternal item)
    {
        return null;
    }
}

partial class GlobalBinaryWriteTranslation
{
    public static partial void WriteBinaryTypeCharCustom(
        MutagenWriter writer,
        IGlobalGetter item)
    {
        if (item.TypeChar != GlobalFloat.TRIGGER_CHAR || item is IGlobalFloatGetter { OutputChar: true })
        {
            CharBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                writer,
                item.TypeChar,
                header: RecordTypes.FNAM);
        }
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
                    package);
            case GlobalShort.TRIGGER_CHAR:
                return GlobalShortBinaryOverlay.GlobalShortFactory(
                    stream,
                    package);
            case GlobalFloat.TRIGGER_CHAR:
            {
                var ret = (GlobalFloatBinaryOverlay)GlobalFloatBinaryOverlay.GlobalFloatFactory(
                    stream,
                    package);
                ret.OutputChar = true;
                return ret;
            }
            case GlobalBool.TRIGGER_CHAR:
                return GlobalBoolBinaryOverlay.GlobalBoolFactory(
                    stream,
                    package);
            default:
                return GlobalFloatBinaryOverlay.GlobalFloatFactory(
                    stream,
                    package);
        }
    }

    public partial ParseResult TypeCharCustomParse(OverlayStream stream, int offset)
    {
        return null;
    }
}

partial class GlobalFloatBinaryOverlay
{
    public bool OutputChar { get; set; }
}
