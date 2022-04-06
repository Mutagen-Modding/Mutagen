using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class PerkScriptFlag
{
    [Flags]
    public enum Flag
    {
        RunImmediately = 0x01,
        ReplaceDefault = 0x02,
    }
}