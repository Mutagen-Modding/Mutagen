using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class CellLighting
    {
        #region IAmbientColorsCommon
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Color DirectionalXPlus { get => AmbientDirectionalXPlus; set => AmbientDirectionalXPlus = value; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Color DirectionalXMinus { get => AmbientDirectionalXMinus; set => AmbientDirectionalXMinus = value; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Color DirectionalYPlus { get => AmbientDirectionalYPlus; set => AmbientDirectionalYPlus = value; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Color DirectionalYMinus { get => AmbientDirectionalYMinus; set => AmbientDirectionalYMinus = value; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Color DirectionalZPlus { get => AmbientDirectionalZPlus; set => AmbientDirectionalZPlus = value; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Color DirectionalZMinus { get => AmbientDirectionalZMinus; set => AmbientDirectionalZMinus = value; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Color Specular { get => AmbientSpecular; set => AmbientSpecular = value; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public float Scale { get => AmbientScale; set => AmbientScale = value; }
        #endregion

        [Flags]
        public enum Inherit
        {
            AmbientColor = 0x0001,
            DirectionalColor = 0x0002,
            FogColor = 0x0004,
            FogNear = 0x0008,
            FogFar = 0x0010,
            DirectionalRotation = 0x0020,
            DirectionalFade = 0x0040,
            ClipDistance = 0x0080,
            FogPower = 0x0100,
            FogMax = 0x0200,
            LightFadeDistances = 0x0400,
        }
    }

    namespace Internals
    {
        public partial class CellLightingBinaryOverlay
        {

            #region IAmbientColorsCommon
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Color DirectionalXPlus => AmbientDirectionalXPlus;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Color DirectionalXMinus => AmbientDirectionalXMinus;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Color DirectionalYPlus => AmbientDirectionalYPlus;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Color DirectionalYMinus => AmbientDirectionalYMinus;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Color DirectionalZPlus => AmbientDirectionalZPlus;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Color DirectionalZMinus => AmbientDirectionalZMinus;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Color Specular => AmbientSpecular;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public float Scale => AmbientScale;
            #endregion
        }
    }
}
