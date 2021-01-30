using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Transform
    {

        [Flags]
        public enum MajorFlag
        {
            AroundOrigin = 0x0000_8000
        }
    }
}
