using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class TalkingActivator
{
    [Flags]
    public enum MajorFlag
    {
        HiddenFromLocalMap = 0x0000_0200,
        RandomAnimStart = 0x0001_0000,
        RadioStation = 0x0002_0000,
    }
}