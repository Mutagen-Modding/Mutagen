using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Ammo
    {
        [Flags]
        public enum AmmoFlag
        {
            IgnoresNormalWeaponResistance = 0x01
        }
    }
}
