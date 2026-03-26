using Noggog;
namespace Mutagen.Bethesda.Fallout76;

public partial class LeveledItem
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
        UseAll = 0x04
    }
}

partial class LeveledItemBinaryOverlay
{
    public ReadOnlyMemorySlice<Byte> BelowVersion => Array.Empty<byte>();
}
