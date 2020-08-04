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
        Oblivion,
        [Description("Skyrim Legendary Edition")]
        SkyrimLE,
        [Description("Skyrim Special Edition")]
        SkyrimSE
    }
}
