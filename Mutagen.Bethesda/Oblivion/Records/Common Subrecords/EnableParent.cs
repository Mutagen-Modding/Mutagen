using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class EnableParent
    {
        [Flags]
        public enum Flag
        {
            SetEnableStateToOppositeOfParent = 0x01
        }
    }
}
