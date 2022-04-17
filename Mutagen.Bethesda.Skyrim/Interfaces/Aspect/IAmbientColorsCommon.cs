using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

/// <summary>
/// Interface for ambient color definitions.
/// Implemented by: [AmbientColors, CellLighting]
/// </summary>
public interface IAmbientColorsCommon : IAmbientColorsCommonGetter
{
    new Color DirectionalXPlus { get; set; }
    new Color DirectionalXMinus { get; set; }
    new Color DirectionalYPlus { get; set; }
    new Color DirectionalYMinus { get; set; }
    new Color DirectionalZPlus { get; set; }
    new Color DirectionalZMinus { get; set; }
    new Color Specular { get; set; }
    new Single Scale { get; set; }
}

/// <summary>
/// Interface for ambient color definitions.
/// Implemented by: [AmbientColors, CellLighting]
/// </summary>
public interface IAmbientColorsCommonGetter
{
    Color DirectionalXPlus { get; }
    Color DirectionalXMinus { get; }
    Color DirectionalYPlus { get; }
    Color DirectionalYMinus { get; }
    Color DirectionalZPlus { get; }
    Color DirectionalZMinus { get; }
    Color Specular { get; }
    Single Scale { get; }
}