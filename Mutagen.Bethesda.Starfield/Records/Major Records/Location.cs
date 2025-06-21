namespace Mutagen.Bethesda.Starfield;

partial class Location
{
    [Flags]
    public enum MajorFlag
    {
        InteriorCellUseRefLocationForWorldMapPlayerMarker = 0x0800,
        OffLimits = 0x0002_0000,
        CannotWait = 0x0008_0000,
        PublicArea = 0x0010_0000,
    }
}