using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    [Flags]
    public enum ClassFlag
    {
        Playable = 0x1,
        Guard = 0x2
    }
}
