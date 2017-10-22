using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public partial class Hair
    {
        [Flags]
        public enum HairFlag
        {
            Playable = 0x01,
            NotMale = 0x02,
            NotFemale = 0x04,
            Fixed = 0x08
        }
    }
}
