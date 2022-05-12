namespace Mutagen.Bethesda.Oblivion;

public partial class MapMarker
{
    [Flags]
    public enum Flag
    {
        Visible = 0x01,
        CanTravelTo = 0x02
    }

    public enum Type
    {
        None = 0,
        Camp = 1,
        Cave = 2,
        City = 3,
        ElvenRuin = 4,
        FortRuin = 5,
        Mine = 6,
        Landmark = 7,
        Tavern = 8,
        Settlement = 9,
        DaedricShrine = 10,
        OblivionGate = 11,
        UnknownDoorIcon = 12,
    }
}