using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Condition
    {
        public enum Flag
        {
            OR = 0x01,
            RunOnTarget = 0x02,
            UseGlobal = 0x04
        }
    }
}
