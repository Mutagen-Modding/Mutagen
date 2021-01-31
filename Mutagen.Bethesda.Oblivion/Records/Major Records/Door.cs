using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Door
    {
        [Flags]
        public enum DoorFlag
        {
            OblivionGate = 0x01,
            AutomaticDoor = 0x02,
            Hidden = 0x04,
            MinimalUse = 0x08
        }
    }
}
