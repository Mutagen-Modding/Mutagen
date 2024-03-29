﻿namespace Mutagen.Bethesda.Starfield;

partial class MusicType
{
    [Flags]
    public enum Flag
    {
        PlaysOneSelection = 0x01,
        AbruptTransition = 0x02,
        CycleTracks = 0x04,
        MaintainTrackOrder = 0x08,
        DucksCurrentTrack = 0x20,
        DoesNotQueue = 0x40,
    }
}