using System.Diagnostics;
using System.Drawing;

namespace Mutagen.Bethesda.Skyrim;

public partial class LightingTemplate
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
}

partial class LightingTemplateBinaryOverlay
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