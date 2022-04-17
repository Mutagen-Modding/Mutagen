using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class HeadPart
{
    [Flags]
    public enum Flag
    {
        Playable = 0x01,
        Male = 0x02,
        Female = 0x04,
        IsExtraPart = 0x08,
        UseSolidTint = 0x10,
    }

    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
    }

    public enum TypeEnum
    {
        Misc,
        Face,
        Eyes,
        Hair,
        FacialHair,
        Scars,
        Eyebrows,
    }
}