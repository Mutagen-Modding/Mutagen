namespace Mutagen.Bethesda.Fallout3;

public partial class Package
{
    public enum Types
    {
        Find = 0,
        Follow = 1,
        Escort = 2,
        Eat = 3,
        Sleep = 4,
        Wander = 5,
        Travel = 6,
        Accompany = 7,
        UseItemAt = 8,
        Ambush = 9,
        FleeNotCombat = 10,
        Sandbox = 12,
        Patrol = 13,
        Guard = 14,
        Dialogue = 15,
        UseWeapon = 16,
    }

    [Flags]
    public enum Flag : uint
    {
        OffersServices = 0x0000_0001,
        MustComplete = 0x0000_0004,
        MaintainSpeedAtGoal = 0x0000_0008,
        UnlockDoorsAtPackageStart = 0x0000_0040,
        UnlockDoorsAtPackageEnd = 0x0000_0080,
        ContinueIfPcNear = 0x0000_0200,
        OncePerDay = 0x0000_0400,
        PreferredSpeed = 0x0000_2000,
        AlwaysSneak = 0x0002_0000,
        AllowSwimming = 0x0004_0000,
        AllowFalls = 0x0008_0000,
        ArmorUnequipped = 0x0010_0000,
        WeaponsUnequipped = 0x0020_0000,
        DefensiveCombat = 0x0040_0000,
        WeaponDrawn = 0x0080_0000,
        NoCombatAlert = 0x0800_0000,
        WearSleepOutfit = 0x2000_0000,
    }

    public enum DayOfWeek
    {
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Weekdays = 7,
        Weekends = 8,
        MondayWednesdayFriday = 9,
        TuesdayThursday = 10,
        Any = 255,
    }
}
