namespace Mutagen.Bethesda.Fallout3;

public enum MediaSetType
{
    NoSet = -1,
    BattleSet = 0,
    LocationSet = 1,
    DungeonSet = 2,
    IncidentalSet = 3,
}

[Flags]
public enum MediaSetEnableFlag
{
    DayOuter = 0x01,
    DayMiddle = 0x02,
    DayInner = 0x04,
    NightOuter = 0x08,
    NightMiddle = 0x10,
    NightInner = 0x20,
}
