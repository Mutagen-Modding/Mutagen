using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Light
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
            RandomAnimStart = 0x0001_0000,
            PortalStrict = 0x0002_0000,
            Obstacle = 0x0200_0000
        }

        [Flags]
        public enum Flag
        {
            Dynamic = 0x0001,
            CanBeCarried = 0x0002,
            Negative = 0x0004,
            Flicker = 0x0008,
            OffByDefault = 0x0020,
            FlickerSlow = 0x0040,
            Pulse = 0x0080,
            PulseSlow = 0x0100,
            SpotLight = 0x0200,
            ShadowSpotlight = 0x0400,
            ShadowHemisphere = 0x0800,
            ShadowOmnidirectional = 0x1000,
            PortalStrict = 0x2000,
        }
    }
}
