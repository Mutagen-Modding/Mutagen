using Mutagen.Bethesda.Plugins;
using System;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout76;

partial class Challenge
{
    [Flags]
    public enum MajorFlag
    {
    }

    public enum ChallengeFrequency
    {
        Daily = 0,
        Weekly = 1,
        Lifetime = 2,
        Monthly = 3,
        Event = 4,
        Seasonal = 5,
    }

    public enum ChallengeCategory
    {
        Character = 0,
        Survival = 1,
        Exploration = 2,
        Pets = 3,
        Combat = 4,
        Social = 5,
        World = 6,
        Tracker = 7,
        SubChallengeUnsorted = 8,
        Fishing = 9,
        BurningSprings = 10,
    }
}

partial class ChallengeBinaryOverlay
{
}
