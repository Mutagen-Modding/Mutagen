using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Weapon
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ObjectBounds? IObjectBoundedOptional.ObjectBounds
        {
            get => this.ObjectBounds;
            set => this.ObjectBounds = value ?? new ObjectBounds();
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter? IObjectBoundedOptionalGetter.ObjectBounds => this.ObjectBounds;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IIconsGetter? IHasIconsGetter.Icons => this.Icons;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x0000_0004
        }
    }
}
