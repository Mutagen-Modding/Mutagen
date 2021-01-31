using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class TalkingActivator
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            HiddenFromLocalMap = 0x0000_0200,
            RandomAnimStart = 0x0001_0000,
            RadioStation = 0x0002_0000,
        }
    }
}
