using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Oblivion;

public partial class Condition
{
    [Flags]
    public enum Flag
    {
        OR = 0x01,
        RunOnTarget = 0x02,
        UseGlobal = 0x04
    }
        
    static Condition CustomRecordTypeTrigger(
        MutagenFrame frame,
        RecordType recordType, 
        TypedParseParams? translationParams)
    {
        var pos = frame.PositionWithOffset;
        var span = frame.ReadSpan(0x1A);
        byte[] newBytes = new byte[span.Length + 4];
        span.CopyTo(newBytes.AsSpan());
        newBytes[4] = 0x18;
        newBytes[3] = (byte)'A';
        LoquiBinaryTranslation<Condition>.Instance.Parse(
            frame: new MutagenFrame(new MutagenMemoryReadStream(newBytes, frame.MetaData, offsetReference: pos)),
            item: out var item,
            translationParams: translationParams);
        return item;
    }
}
    
partial class ConditionBinaryCreateTranslation
{
    public const byte Mask = 0xF0;

    public static Condition.Flag GetFlag(byte b)
    {
        return (Condition.Flag)(0xF & b);
    }

    public static CompareOperator GetCompareOperator(byte b)
    {
        return (CompareOperator)((Mask & b) >> 4);
    }

    public static partial void FillBinaryInitialParserCustom(MutagenFrame frame, ICondition item)
    {
        byte b = frame.ReadUInt8();
        item.Flags = GetFlag(b);
        item.CompareOperator = GetCompareOperator(b);
    }
}

partial class ConditionBinaryWriteTranslation
{
    public static partial void WriteBinaryInitialParserCustom(MutagenWriter writer, IConditionGetter item)
    {
        byte b = (byte)item.Flags;
        b |= (byte)(((int)(item.CompareOperator) * 16) & ConditionBinaryCreateTranslation.Mask);
        writer.Write(b);
    }
}

partial class ConditionBinaryOverlay
{
    public Condition.Flag Flags => ConditionBinaryCreateTranslation.GetFlag(_data.Span[0]);
    public CompareOperator CompareOperator => ConditionBinaryCreateTranslation.GetCompareOperator(_data.Span[0]);

    static ConditionBinaryOverlay CustomRecordTypeTrigger(
        OverlayStream stream,
        RecordType recordType,
        BinaryOverlayFactoryPackage package,
        TypedParseParams? parseParams)
    {
        var rawBytes = stream.ReadSpan(0x1A);
        byte[] newBytes = new byte[rawBytes.Length + 4];
        rawBytes.CopyTo(newBytes.AsSpan());
        newBytes[4] = 0x18;
        newBytes[3] = (byte)'A';
        return ConditionBinaryOverlay.ConditionFactory(
            stream: new OverlayStream(newBytes, package),
            package: package,
            parseParams: parseParams);
    }
}
