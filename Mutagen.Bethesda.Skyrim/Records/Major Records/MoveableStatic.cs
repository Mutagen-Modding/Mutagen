using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MoveableStatic
    {
        [Flags]
        public enum MajorFlag
        {
            MustUpdateAnims = 0x0000_0100,
            HiddenFromLocalMap = 0x0000_0200,
            HasDistantLOD = 0x0000_8000,
            RandomAnimStart = 0x0001_0000,
            HasCurrents = 0x0008_0000,
            Obstacle = 0x0200_0000,
            NavMeshGenerationFilter = 0x0400_0000,
            NavMeshGenerationBoundingBox = 0x0800_0000,
            NavMeshGenerationGround = 0x4000_0000,
        }

        [Flags]
        public enum Flag
        {
            OnLocalMap = 0x01
        }
    }
}
