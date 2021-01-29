using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Specific game releases
    /// </summary>
    public enum GameRelease
    {
        [Description("Oblivion")]
        Oblivion = 0,
        [Description("Skyrim Legendary Edition")]
        SkyrimLE = 1,
        [Description("Skyrim Special Edition")]
        SkyrimSE = 2,
        [Description("Skyrim VR")]
        SkyrimVR = 3,
        [Description("Fallout 4")]
        Fallout4 = 4
    }
}
