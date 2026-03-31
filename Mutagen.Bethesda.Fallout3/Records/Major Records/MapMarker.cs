namespace Mutagen.Bethesda.Fallout3;

public partial class MapMarker
{
    [Flags]
    public enum Flag
    {
        Visible = 0x01,
        CanTravelTo = 0x02,
        ShowAllIsHidden = 0x04
    }

    public enum MarkerType
    {
        None = 0,
        City = 1,
        Settlement = 2,
        Encampment = 3,
        NaturalLandmark = 4,
        Cave = 5,
        Factory = 6,
        Monument = 7,
        Military = 8,
        Office = 9,
        TownRuins = 10,
        UrbanRuins = 11,
        SewerRuins = 12,
        Metro = 13,
        Vault = 14,
    }
}
