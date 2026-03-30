using System;

namespace Mutagen.Bethesda.Fallout3;

public partial class MiscItem
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
    }
}
 