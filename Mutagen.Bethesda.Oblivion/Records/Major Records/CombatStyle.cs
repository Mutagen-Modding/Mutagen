using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Oblivion;

public partial class CombatStyle
{
    [Flags]
    public enum Flag
    {
        Advanced = 0x0001,
        ChooseAttackUsingPercentChance = 0x0002,
        IgnoreAlliesInArea = 0x0004,
        WillYield = 0x0008,
        RejectsYields = 0x0010,
        FleeingDisabled = 0x0020,
        PrefersRanged = 0x0040,
        MeleeAlertOK = 0x0080,
        DoNotAcquire = 0x0100,
    }
}

partial class CombatStyleDataBinaryCreateTranslation
{
    public static partial void FillBinarySecondaryFlagsCustom(MutagenFrame frame, ICombatStyleData item)
    {
        int flags = frame.ReadInt32();
        var otherFlag = (CombatStyle.Flag)(flags << 8);
        item.Flags |= otherFlag;
    }
}

partial class CombatStyleDataBinaryWriteTranslation
{
    public static partial void WriteBinarySecondaryFlagsCustom(MutagenWriter writer, ICombatStyleDataGetter item)
    {
        int flags = (int)item.Flags;
        flags >>= 8;
        writer.Write(flags);
    }
}

partial class CombatStyleDataBinaryOverlay
{
    private bool GetFlagsIsSetCustom() => true;
    public partial CombatStyle.Flag GetFlagsCustom(int location)
    {
        var ret = (CombatStyle.Flag)_structData[0x50];
        if (!this.Versioning.HasFlag(CombatStyleData.VersioningBreaks.Break4))
        {
            int flags = BinaryPrimitives.ReadInt32LittleEndian(_structData.Span.Slice(0x78));
            var otherFlag = (CombatStyle.Flag)(flags << 8);
            ret |= otherFlag;
        }
        return ret;
    }
}