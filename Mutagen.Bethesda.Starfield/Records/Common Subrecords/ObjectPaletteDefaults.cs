namespace Mutagen.Bethesda.Starfield;

public partial class ObjectPaletteDefaults
{
    [Flags]
    public enum Flag
    {
        ConformToSlope = 0x00001,
        RotateZToSlope = 0x00004,
        SlopLimitIsMinSlope = 0x00008,
        PlaceAsMarker = 0x00010,
        OutsideWaterHeight = 0x00020,
    }

    public enum FootprintSizes
    {
        XLarge,
        Large,
        Medium,
        Small,
        None
    } 
}