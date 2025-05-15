using System.ComponentModel;

namespace Mutagen.Bethesda;

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
    [Description("Enderal LE")]
    EnderalLE = 5,
    [Description("Enderal SE")]
    EnderalSE = 6,
    [Description("Fallout 4")]
    Fallout4 = 4,
    [Description("Skyrim Special Edition GOG")]
    SkyrimSEGog = 7,
    [Description("Starfield")]
    Starfield = 8,
    [Description("Fallout 4 VR")]
    Fallout4VR = 9,
    [Description("Oblivion Remastered")]
    OblivionRE = 10,
    [Description("Fallout 3")]
    Fallout3 = 11,
    [Description("Fallout New Vegas")]
    FalloutNV = 12,
}