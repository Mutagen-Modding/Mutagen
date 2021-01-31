using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Activator
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            HasTreeLOD = 0x0000_0040,
            MustUpdateAnims = 0x0000_0100,
            HiddenFromLocalMap = 0x0000_0200,
            HasDistantLOD = 0x0000_8000,
            RandomAnimStart = 0x0001_0000,
            Dangerous = 0x0002_0000,
            IgnoreObjectInteraction = 0x0010_0000,
            IsMarker = 0x0080_0000,
            Obstacle = 0x0200_0000,
            NavMeshGenerationFilter = 0x0400_0000,
            NavMeshGenerationBoundingBox = 0x0800_0000,
            ChildCanUse = 0x2000_0000,
            NavMeshGenerationGround = 0x4000_0000,
        }

        [Flags]
        public enum Flag
        {
            NoDisplacement = 0x01,
            IgnoredBySandbox = 0x02,
        }
    }
}
