using System;

namespace Mutagen.Bethesda.Fallout3;

[Flags]
public enum ChallengeFlag : uint
{
    StartDisabled = 0x00000001,
    Recurring = 0x00000002,
    ShowZeroProgress = 0x00000004,
}
