using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    /// <summary>
    /// An enum for the slots in a biped body in Oblivion
    /// </summary>
    [Flags]
    public enum BipedFlag
    {
        Head = 0x0001,
        Hair = 0x0002,
        UpperBody = 0x0004,
        LowerBody = 0x0008,
        Hand = 0x0010,
        Foot = 0x0020,
        RightRing = 0x0040,
        LeftRing = 0x0080,
        Amulet = 0x0100,
        Weapon = 0x0200,
        BackWeapon = 0x0400,
        SideWeapon = 0x0800,
        Quiver = 0x1000,
        Shield = 0x2000,
        Torch = 0x4000,
        Tail = 0x8000
    }
}
