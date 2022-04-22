using System.ComponentModel;

namespace Mutagen.Bethesda;

/// <summary>
/// Game categories that generally have similar or the same formats
/// </summary>
public enum GameCategory
{
    [Description("Oblivion")]
    Oblivion,
    [Description("Skyrim")]
    Skyrim, 
    [Description("Fallout4")]
    Fallout4,
}