using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Fallout3;

public partial class Condition
{
    [Flags]
    public enum Flag
    {
        OR = 0x01,
        UseAliases = 0x02,
        UseGlobal = 0x04,
        UsePackData = 0x08,
        SwapSubjectAndTarget = 0x10
    }

    public enum RunOn
    {
        Subject = 0,
        Target = 1,
        Reference = 2,
        CombatTarget = 3,
        LinkedReference = 4,
    }
}

partial class ConditionBinaryCreateTranslation
{
    public const byte Mask = 0xF0;

    public static Condition.Flag GetFlag(byte b)
    {
        return (Condition.Flag)(0x1F & b);
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
    public Condition.Flag Flags => ConditionBinaryCreateTranslation.GetFlag(_structData.Span[0]);
    public CompareOperator CompareOperator => ConditionBinaryCreateTranslation.GetCompareOperator(_structData.Span[0]);
}
