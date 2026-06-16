using System;

namespace Mutagen.Bethesda.Fallout3;

public partial class Challenge
{
    public enum ChallengeType
    {
        KillFromFormList = 0,
        KillSpecificFormID = 1,
        KillAnyInCategory = 2,
        HitEnemy = 3,
        DiscoverMapMarker = 4,
        UseItem = 5,
        AcquireItem = 6,
        UseSkill = 7,
        DoDamage = 8,
        UseItemFromList = 9,
        AcquireItemFromList = 10,
        MiscellaneousStat = 11,
        CraftUsingItem = 12,
        ScriptedChallenge = 13,
    }

    [Flags]
    public enum ChallengeFlag
    {
        StartDisabled = 0x00000001,
        Recurring = 0x00000002,
        ShowZeroProgress = 0x00000004,
    }
}
