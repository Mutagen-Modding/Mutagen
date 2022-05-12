namespace Mutagen.Bethesda.Skyrim;

public partial class WorldspaceParent
{
    [Flags]
    public enum Flag
    {
        UseLandData = 0x0001,
        UseLodData = 0x0002,
        UseMapData = 0x0004,
        UseWaterData = 0x0008,
        UseClimateData = 0x0010,
        UseImageSpaceData = 0x0020,
        UseSkyCell = 0x0040,
    }
}