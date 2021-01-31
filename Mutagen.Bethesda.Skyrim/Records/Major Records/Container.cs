using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Container
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            HasDistantLOD = 0x0000_8000,
            RandomAnimStart = 0x0001_0000,
            Obstacle = 0x0200_0000,
            NavMeshGenerationFilter = 0x0400_0000,
            NavMeshGenerationBoundingBox = 0x0800_0000,
            NavMeshGenerationGround = 0x4000_0000
        }

        [Flags]
        public enum Flag
        {
            AllowSoundsWhenAnimation = 0x01,
            Respawns = 0x02,
            ShowOwner = 0x04
        }
    }
}
