using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Container
    {
        [Flags]
        public enum ContainerFlag
        {
            Respawns = 0x2,
        }
    }
}
