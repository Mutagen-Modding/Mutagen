using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SceneAction
    {
        public enum TypeEnum
        {
            Dialog,
            Package,
            Timer,
        }

        [Flags]
        public enum Flag : uint
        {
            FaceTarget = 0x8000,
            Looping = 0x0001_0000,
            HeadtrackPlayer = 0x0002_0000,
        }
    }
}
