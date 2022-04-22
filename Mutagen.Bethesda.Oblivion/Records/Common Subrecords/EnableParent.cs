using System;

namespace Mutagen.Bethesda.Oblivion;

public partial class EnableParent
{
    [Flags]
    public enum Flag
    {
        SetEnableStateToOppositeOfParent = 0x01
    }
}