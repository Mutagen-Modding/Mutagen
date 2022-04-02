using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class MovableStatic
    {
        [Flags]
        public enum MajorFlag
        {
            MustUpdateAnims = 0x0000_0100,
            HiddenFromLocalMap = 0x0000_0200,
            UsedAsPlatform = 0x0000_0800,
            PackInUseOnly = 0x0000_2000,
            HasDistantLod = 0x0000_8000,
            RandomAnimStart = 0x0001_0000,
            HasCurrents = 0x0008_0000,
            Obstacle = 0x0200_0000,
            NavMeshGenerationFilter = 0x0400_0000,
            NavMeshGenerationBoundingBox = 0x0800_0000,
            NavMeshGenerationGround = 0x4000_0000,
        }
    }
}
