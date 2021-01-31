using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Book
    {
        [Flags]
        public enum BookFlag
        {
            Scroll = 0x01,
            CantBeTaken = 0x02,
        }
    }
}
