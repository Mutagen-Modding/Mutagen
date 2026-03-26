using Noggog;
namespace Mutagen.Bethesda.Fallout76;

public partial class LeveledNpc
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
        CalculateAll = 0x04
    }
}

partial class LeveledNpcBinaryOverlay
{
    public ReadOnlyMemorySlice<Byte> BelowVersion => Array.Empty<byte>();
}
