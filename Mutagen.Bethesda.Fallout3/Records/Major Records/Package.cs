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
        MustReachLocation = 0x0000_0002,
        MustComplete = 0x0000_0004,
        LockDoorsAtPackageStart = 0x0000_0008,
        LockDoorsAtPackageEnd = 0x0000_0010,
        LockDoorsAtLocation = 0x0000_0020,
        UnlockDoorsAtPackageStart = 0x0000_0040,
        UnlockDoorsAtPackageEnd = 0x0000_0080,
        UnlockDoorsAtLocation = 0x0000_0100,
        ContinueIfPcNear = 0x0000_0200,
        OncePerDay = 0x0000_0400,
        SkipFalloutBehavior = 0x0000_1000,
        AlwaysRun = 0x0000_2000,
        AlwaysSneak = 0x0002_0000,
        AllowSwimming = 0x0004_0000,
        AllowFalls = 0x0008_0000,
        HeadTrackingOff = 0x0010_0000,
        WeaponsUnequipped = 0x0020_0000,
        DefensiveCombat = 0x0040_0000,
        WeaponDrawn = 0x0080_0000,
        NoIdleAnims = 0x0100_0000,
        PretendInCombat = 0x0200_0000,
        ContinueDuringCombat = 0x0400_0000,
        NoCombatAlert = 0x0800_0000,
        NoWarnAttackBehavior = 0x1000_0000,
    }

    [Flags]
    public enum ServiceFlag : ushort
    {
        AllowBuying = 0x0100,
        AllowKilling = 0x0200,
        AllowStealing = 0x0400,
    }

    [Flags]
    public enum UseItemAtFlag : ushort
    {
        SitDown = 0x0004,
        AllowBuying = 0x0100,
        AllowKilling = 0x0200,
        AllowStealing = 0x0400,
    }

    [Flags]
    public enum WanderFlag : ushort
    {
        NoEating = 0x0001,
        NoSleeping = 0x0002,
        NoConversation = 0x0004,
        NoIdleMarkers = 0x0008,
        NoFurniture = 0x0010,
        NoWandering = 0x0020,
    }

    [Flags]
    public enum AmbushFlag : ushort
    {
        HideWhileAmbushing = 0x0001,
    }

    [Flags]
    public enum GuardFlag : ushort
    {
        RemainNearReferenceToGuard = 0x0008,
    }

    [Flags]
    public enum BehaviorFlag : ushort
    {
        HellosToPlayer = 0x0001,
        RandomConversations = 0x0002,
        ObserveCombatBehavior = 0x0004,
        ReactionToPlayerActions = 0x0010,
        FriendlyFireComments = 0x0020,
        AggroRadiusBehavior = 0x0040,
        AllowIdleChatter = 0x0080,
        AvoidRadiation = 0x0100,
    }

    public enum LocationType
    {
        NearReference = 0,
        InCell = 1,
        NearCurrentLocation = 2,
        NearEditorLocation = 3,
        ObjectId = 4,
        ObjectType = 5,
        NearLinkedReference = 6,
        AtPackageLocation = 7,
    }

    public enum TargetType
    {
        SpecificReference = 0,
        ObjectId = 1,
        ObjectType = 2,
        LinkedReference = 3,
    }

    public enum ObjectType
    {
        None = 0,
        Activators = 1,
        Armor = 2,
        Books = 3,
        Clothing = 4,
        Containers = 5,
        Doors = 6,
        Ingredients = 7,
        Lights = 8,
        Misc = 9,
        Flora = 10,
        Furniture = 11,
        WeaponsAny = 12,
        Ammo = 13,
        Npcs = 14,
        Creatures = 15,
        Keys = 16,
        Alchemy = 17,
        Food = 18,
        AllCombatWearable = 19,
        AllWearable = 20,
        WeaponsRanged = 21,
        WeaponsMelee = 22,
        WeaponsNone = 23,
        ActorEffectsAny = 24,
        ActorEffectsRangeTarget = 25,
        ActorEffectsRangeTouch = 26,
        ActorEffectsRangeSelf = 27,
        ActorsAny = 29,
    }

    [Flags]
    public enum DialogueFlag : uint
    {
        NoHeadtracking = 0x0000_0001,
        DontControlTargetMovement = 0x0000_0100,
    }

    public enum DialogueType
    {
        Conversation = 0,
        SayTo = 1,
    }

    [Flags]
    public enum WeaponFlag : uint
    {
        AlwaysHit = 0x0000_0001,
        DoNoDamage = 0x0000_0100,
        CrouchToReload = 0x0001_0000,
        HoldFireWhenBlocked = 0x0100_0000,
    }

    public enum FireRate
    {
        AutoFire = 0,
        VolleyFire = 1,
    }

    public enum FireCount
    {
        NumberOfBursts = 0,
        RepeatFire = 1,
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
