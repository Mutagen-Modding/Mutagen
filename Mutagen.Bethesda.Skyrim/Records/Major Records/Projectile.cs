using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Projectile
    {
        [Flags]
        public enum Flag
        {
            Hitscan = 0x0001,
            Explosion = 0x0002,
            AltTrigger = 0x0004,
            MuzzleFlash = 0x0008,
            CanBeDisabled = 0x0020,
            CanBePickedUp = 0x0040,
            Supersonic = 0x0080,
            PinsLimbs = 0x0100,
            PassThroughSmallTransparent = 0x0200,
            DisableCombatAimCorrection = 0x0400,
            Rotation = 0x08000,
        }

        public enum TypeEnum
        {
            Missile = 0x01,
            Lobber = 0x02,
            Beam = 0x04,
            Flame = 0x08,
            Cone = 0x10,
            Barrier = 0x20,
            Arrow = 0x40,
        }
    }
}
