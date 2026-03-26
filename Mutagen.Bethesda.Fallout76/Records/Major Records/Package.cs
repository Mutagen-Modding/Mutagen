using Noggog;
using System;

namespace Mutagen.Bethesda.Fallout76;

partial class Package
{
    public enum Types
    {
        Package = 18,
        PackageTemplate = 19
    }

    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag : uint
    {
        OffersServices = 0x0000_0001,
        MustComplete = 0x0000_0004,
        MaintainSpeedAtGoal = 0x0000_0008,
        TreatAsPlayerFollower = 0x0000_0010,
        UnlockDoorsAtPackageStart = 0x0000_0040,
        UnlockDoorsAtPackageEnd = 0x0000_0080,
        RequestBlockIdles = 0x0000_0100,
        ContinueIfPcNear = 0x0000_0200,
        OncePerDay = 0x0000_0400,
        SkipLoadIntoFurniture = 0x0000_1000,
        PreferredSpeed = 0x0000_2000,
        AlwaysSneak = 0x0002_0000,
        AllowSwimming = 0x0004_0000,
        IgnoreCombat = 0x0010_0000,
        WeaponsUnequipped = 0x0020_0000,
        WeaponDrawn = 0x0080_0000,
        NoCombatAlert = 0x0800_0000,
        WearSleepOutfit = 0x2000_0000
    }

    public enum Interrupt
    {
        None,
        Spectator,
        ObserveDead,
        GuardWarn,
        Combat,
        CommandTravel,
        CommandActivate,
        LeaveWorkstation,
    }

    public enum Speed
    {
        Walk,
        Jog,
        Run,
        FastWalk,
    }

    [Flags]
    public enum InterruptFlag
    {
        HellosToPlayer = 0x0001,
        RandomConversations = 0x0002,
        ObserveCombatBehavior = 0x0004,
        GreetCorpseBehavior = 0x0008,
        ReactionToPlayerActions = 0x0010,
        FriendlyFireComments = 0x0020,
        AggroRadiusBehavior = 0x0040,
        AllowIdleChatter = 0x0080,
        WorldInteractions = 0x0200,
        OffForImportantScene = 0x0400,
    }

    public enum DayOfWeek : byte
    {
        Any = 0xFF,
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
    }
}

partial class PackageBinaryOverlay
{
    public ReadOnlyMemorySlice<Byte> IsFO3 => Array.Empty<byte>();
    public ReadOnlyMemorySlice<Byte> IsSF1 => Array.Empty<byte>();
}
