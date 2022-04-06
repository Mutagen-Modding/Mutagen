using System;

namespace Mutagen.Bethesda.Skyrim;

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
        Town = 2,
        Settlement = 3,
        Cave = 4,
        Camp = 5,
        Fort = 6,
        NordicRuins = 7,
        DwemerRuin = 8,
        Shipwreck = 9,
        Grove = 10,
        Landmark = 11,
        DragonLair = 12,
        Farm = 13,
        WoodMill = 14,
        Mine = 15,
        ImperialCamp = 16,
        StormcloakCamp = 17,
        Doomstone = 18,
        WheatMill = 19,
        Smelter = 20,
        Stable = 21,
        ImperialTower = 22,
        Clearing = 23,
        Pass = 24,
        Altar = 25,
        Rock = 26,
        Lighthouse = 27,
        OrcStronghold = 28,
        GiantCamp = 29,
        Shack = 30,
        NordicTower = 31,
        NordicDwelling = 32,
        Docks = 33,
        Shrine = 34,
        RiftenCastle = 35,
        RiftenCapitol = 36,
        WindhelmCastle = 37,
        WindhelmCapitol = 38,
        WhiterunCastle = 39,
        WhiterunCapitol = 40,
        SolitudeCastle = 41,
        SolitudeCapitol = 42,
        MarkarthCastle = 43,
        MarkarthCapitol = 44,
        WinterholdCastle = 45,
        WinterholdCapitol = 46,
        MorthalCastle = 47,
        MorthalCapitol = 48,
        FalkreathCastle = 49,
        FalkreathCapitol = 50,
        DawnstarCastle = 51,
        DawnstarCapitol = 52,
        TempleOfMiraak = 53,
        RavenRock = 54,
        BeastStone = 55,
        TelMithryn = 56,
        ToSkyrim = 57,
        ToSolstheim = 58,
    }
}