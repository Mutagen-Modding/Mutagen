namespace Mutagen.Bethesda.Starfield;

partial class PlacedObjectMapMarker
{
    [Flags]
    public enum Flag
    {
        Visible = 0x01,
        CanTravelTo = 0x02,
        ShowAllIsHidden = 0x04,
        UseLocationName = 0x08
    }
}