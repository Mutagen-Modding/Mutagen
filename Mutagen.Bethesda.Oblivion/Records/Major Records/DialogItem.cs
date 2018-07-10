using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class DialogItem
    {
        public enum Flag
        {
            Goodbye = 0x001,
            Random = 0x002,
            SayOnce = 0x004,
            RunImmediately = 0x008,
            InfoRefusal = 0x010,
            RandomEnd = 0x020,
            RunForRumors = 0x040
        }
    }
}
