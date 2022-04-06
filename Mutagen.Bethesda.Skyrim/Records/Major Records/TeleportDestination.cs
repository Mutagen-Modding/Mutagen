using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class TeleportDestination
{
    [Flags]
    public enum Flag
    {
        NoAlarm = 0x1
    }
}