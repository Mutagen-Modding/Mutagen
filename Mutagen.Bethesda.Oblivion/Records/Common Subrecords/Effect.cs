using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Oblivion;

public partial class Effect
{
    public enum EffectType
    {
        Self = 0,
        Touch = 1,
        Target = 2
    }
}

partial class EffectBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryEffectInitialCustom(MutagenFrame frame, IEffect item, PreviousParse lastParsed)
    {
        var subMeta = frame.ReadSubrecordHeader();
        if (subMeta.ContentLength != Mutagen.Bethesda.Plugins.Internals.Constants.HeaderLength)
        {
            throw new ArgumentException($"Magic effect name must be length 4.  Was: {subMeta.ContentLength}");
        }
        var magicEffName = frame.ReadMemory(4);

        if (!frame.Reader.TryGetSubrecordHeader(RecordTypes.EFIT, out var efitMeta))
        {
            throw new ArgumentException("Expected EFIT header.");
        }
        if (efitMeta.ContentLength < Mutagen.Bethesda.Plugins.Internals.Constants.HeaderLength)
        {
            throw new ArgumentException($"Magic effect ref length was less than 4.  Was: {efitMeta.ContentLength}");
        }
        var magicEffName2 = frame.GetMemory(amount: Mutagen.Bethesda.Plugins.Internals.Constants.HeaderLength, offset: efitMeta.HeaderLength);
        if (!magicEffName.Span.SequenceEqual(magicEffName2.Span))
        {
            throw new ArgumentException($"Magic effect names did not match. {BinaryStringUtility.ToZString(magicEffName, frame.MetaData.Encodings.NonTranslated)} != {BinaryStringUtility.ToZString(magicEffName2, frame.MetaData.Encodings.NonTranslated)}");
        }

        return lastParsed.ParsedIndex;
    }
}

partial class EffectBinaryWriteTranslation
{
    public static partial void WriteBinaryEffectInitialCustom(MutagenWriter writer, IEffectGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.EFID))
        {
            RecordTypeBinaryTranslation.Instance.Write(
                writer: writer,
                item: item.Data.MagicEffect);
        }
    }
}

partial class EffectBinaryOverlay
{
    public partial ParseResult EffectInitialCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        return lastParsed;
    }
}