using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Grass
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        #endregion

        public enum UnitsFromWaterTypeEnum
        {
            AboveAtLeast,
            AboveAtMost,
            BelowAtLeast,
            BelowAtMost,
            EitherAtLeast,
            EitherAtMost,
            EitherAtMostAbove,
            EitherAtMostBelow,
        }

        [Flags]
        public enum Flag
        {
            VertexLighting = 0x01,
            UniformScaling = 0x02,
            FitToSlope = 0x04,
        }
    }
}
