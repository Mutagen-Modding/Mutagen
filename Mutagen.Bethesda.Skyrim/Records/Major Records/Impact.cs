using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Impact
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        #endregion

        [Flags]
        public enum Flag
        {
            NoDecalData = 0x01
        }

        public enum ResultType
        {
            Default,
            Destroy,
            Bounce,
            Impale,
            Stick
        }

        public enum OrientationType
        {
            SurfaceNormal,
            ProjectileVector,
            ProjectileReflection,
        }
    }
}
