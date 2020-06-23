using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class VisualEffect
    {
        [Flags]
        public enum Flag
        {
            RotateToFaceTarget = 0x0001,
            AttachToCamera = 0x0002,
            InheritRotation = 0x0004,
        }
    }
}
