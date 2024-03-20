namespace Mutagen.Bethesda.Starfield;

partial class Location
{
    [Flags]
    public enum MajorFlag
    {
        InteriorCellUseRefLocationForWorldMapPlayerMarker = 0x0800,
        PartialForm = 0x4000,
    }
}