using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class Message
{
    [Flags]
    public enum Flag
    {
        MessageBox = 0x01,
        AutoDisplay = 0x02,
    }
}